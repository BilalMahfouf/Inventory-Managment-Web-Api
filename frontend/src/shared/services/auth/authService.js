import {
  api,
  tokenManager,
  extractErrorMessage,
  extractAccessToken,
} from '@shared/services/api/api';

const API_URL = '/auth';

async function _login(request) {
  try {
    const { data } = await api.post(`${API_URL}/login`, request, {
      skipAuthRefresh: true,
    });

    const token = extractAccessToken(data);
    if (!token) {
      return {
        success: false,
        error: 'invalid login response',
      };
    }

    tokenManager.setAccessToken(token);
    return { success: true, data };
  } catch (error) {
    return {
      success: false,
      error: extractErrorMessage(error, 'unknown error'),
    };
  }
}

const authService = {
  getAccessToken: () => tokenManager.getAccessToken(),

  setTokens: newAccessToken => {
    tokenManager.setAccessToken(newAccessToken);
  },

  login: async request => await _login(request),

  logout: async () => {
    try {
      await api.post(`${API_URL}/logout`, {}, { skipAuthRefresh: true });
    } finally {
      tokenManager.clearAccessToken();
      window.location.assign('/');
    }
  },

  refreshToken: async () => {
    try {
      const newToken = await tokenManager.refreshAccessToken();
      return { success: true, data: newToken };
    } catch (error) {
      return {
        success: false,
        error: extractErrorMessage(error, 'failed to refresh token'),
      };
    }
  },
};

export { authService };
