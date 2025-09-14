const API_URL = "/api/auth";
let accessToken = null;

// ========= PRIVATE HELPERS =========
async function _login(request) {
    try {
        const response = await fetch(`${API_URL}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(request),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            return { success: false, error: errorMessage || "unknown error" };
        }

        const data = await response.json();
        authService.setTokens(data.token, data.refreshToken);
        return { success: true, data };
    } catch (e) {
        return { success: false, error: e.message };
    }
}

async function _refreshToken() {
    try {
        const refreshToken = localStorage.getItem("refreshToken");
        if (!refreshToken) {
            return { success: false, error: "refresh token is not available" };
        }

        const response = await fetch(`${API_URL}/refresh-token`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ refreshToken }),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            return { success: false, error: errorMessage || "failed to refresh token" };
        }

        const { accessToken: newAccessToken, newRefreshToken } = await response.json();
        authService.setTokens(newAccessToken, newRefreshToken);

        return { success: true, data: newAccessToken };
    } catch (e) {
        return { success: false, error: e.message };
    }
}

// ========= AUTH SERVICE =========
const authService = {
    getAccessToken: () => accessToken,

    setTokens: (newAccessToken, refreshToken) => {
        accessToken = newAccessToken;
        if (refreshToken) {
            localStorage.setItem("refreshToken", refreshToken);
        }
    },

    getRefreshToken: () => localStorage.getItem("refreshToken"),

    login: async (request) => await _login(request),

    logout: () => {
        accessToken = null;
        localStorage.removeItem("refreshToken");
    },

    refreshToken: async () => await _refreshToken(),
};

// ========= FETCH WITH AUTH =========
async function fetchWithAuth(url, options = {}) {
    if (!accessToken) {
        return { success: false, error: "no access token available" };
    }

    try {
        let response = await fetch(url, {
            ...options,
            headers: {
                ...options.headers,
                Authorization: `Bearer ${accessToken}`,
            },
        });

        // handle 401 -> refresh & retry
        if (response.status === 401) {
            const refreshResult = await authService.refreshToken();
            if (!refreshResult.success) {
                return { success: false, error: refreshResult.error };
            }

            response = await fetch(url, {
                ...options,
                headers: {
                    ...options.headers,
                    Authorization: `Bearer ${refreshResult.data}`, // new token
                },
            });
        }

        if (!response.ok) {
            const text = await response.text();
            return { success: false, error: text || "Request failed" };
        }

        return { success: true, response };
    } catch (e) {
        return { success: false, error: e.message };
    }
}

export { authService, fetchWithAuth };
