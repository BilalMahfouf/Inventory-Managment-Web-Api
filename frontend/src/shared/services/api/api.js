import axios from 'axios';

const AUTH_LOGIN_URL = '/auth/login';
const AUTH_REFRESH_URL = '/auth/refresh-token';
const AUTH_LOGOUT_URL = '/auth/logout';

let accessToken = null;
let refreshPromise = null;

const api = axios.create({
  baseURL: '/api',
  timeout: 10000,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

function extractAccessToken(payload) {
  return payload?.accessToken || payload?.token || payload?.value?.token || null;
}

function setTokens(newAccessToken) {
  accessToken = newAccessToken || null;
}

function getAccessToken() {
  return accessToken;
}

function clearTokens() {
  accessToken = null;
}

function extractErrorMessage(error, fallbackMessage) {
  const responseData = error?.response?.data;
  if (typeof responseData === 'string' && responseData.trim() !== '') {
    return responseData;
  }
  if (responseData?.value?.message) {
    return responseData.value.message;
  }
  if (responseData?.message) {
    return responseData.message;
  }
  if (responseData?.title) {
    return responseData.title;
  }
  return error?.message || fallbackMessage;
}

function isAuthRequest(url) {
  if (!url) {
    return false;
  }

  return (
    url.includes(AUTH_LOGIN_URL) ||
    url.includes(AUTH_REFRESH_URL) ||
    url.includes(AUTH_LOGOUT_URL)
  );
}

async function refreshAccessToken() {
  const { data } = await api.post(
    AUTH_REFRESH_URL,
    {},
    {
      skipAuthRefresh: true,
    }
  );

  const newAccessToken = extractAccessToken(data);

  if (!newAccessToken) {
    throw new Error('failed to refresh token');
  }

  setTokens(newAccessToken);
  return newAccessToken;
}

api.interceptors.request.use(config => {
  if (config.skipAuthRefresh) {
    return config;
  }

  if (accessToken) {
    config.headers = config.headers || {};
    config.headers.Authorization = `Bearer ${accessToken}`;
  }

  return config;
});

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error?.config;

    if (!originalRequest) {
      return Promise.reject(error);
    }

    if (
      originalRequest.skipAuthRefresh ||
      originalRequest._retry ||
      error?.response?.status !== 401 ||
      isAuthRequest(originalRequest.url)
    ) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    if (!refreshPromise) {
      refreshPromise = refreshAccessToken()
        .catch(refreshError => {
          clearTokens();
          throw refreshError;
        })
        .finally(() => {
          refreshPromise = null;
        });
    }

    try {
      const newAccessToken = await refreshPromise;
      originalRequest.headers = originalRequest.headers || {};
      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
      return api(originalRequest);
    } catch (refreshError) {
      return Promise.reject(refreshError);
    }
  }
);

export {
  api,
  setTokens,
  getAccessToken,
  clearTokens,
  refreshAccessToken,
  extractErrorMessage,
  extractAccessToken,
};
