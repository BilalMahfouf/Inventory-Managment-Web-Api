import axios from 'axios';

const AUTH_LOGIN_URL = '/auth/login';
const AUTH_REFRESH_URL = '/auth/refresh-token';
const AUTH_LOGOUT_URL = '/auth/logout';
const DEFAULT_BACKEND_ORIGIN = 'https://localhost:7443';
const PUBLIC_ROUTE_PREFIXES = [
  '/auth',
  '/location-type',
  '/stock-movement-types',
];

function normalizeBackendOrigin(origin) {
  if (typeof origin !== 'string') {
    return DEFAULT_BACKEND_ORIGIN;
  }

  const trimmed = origin.trim();
  if (!trimmed) {
    return DEFAULT_BACKEND_ORIGIN;
  }

  return trimmed.replace(/\/+$/, '');
}

const BACKEND_ORIGIN = normalizeBackendOrigin(
  import.meta.env.VITE_API_ORIGIN
);
const API_BASE_URL = `${BACKEND_ORIGIN}/api`;

let accessToken = null;
let refreshPromise = null;
const tokenSubscribers = new Set();
const unauthorizedListeners = new Set();

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  withCredentials: true,
});

function extractAccessToken(payload) {
  if (typeof payload === 'string' && payload.trim() !== '') {
    return payload;
  }

  return payload?.accessToken || payload?.token || payload?.value?.token || null;
}

function setTokens(newAccessToken) {
  accessToken = newAccessToken || null;
  tokenSubscribers.forEach(listener => {
    listener(accessToken);
  });
}

function getAccessToken() {
  return accessToken;
}

function clearTokens({ notifyUnauthorized = false } = {}) {
  accessToken = null;
  tokenSubscribers.forEach(listener => {
    listener(accessToken);
  });

  if (notifyUnauthorized) {
    unauthorizedListeners.forEach(listener => {
      listener();
    });
  }
}

function subscribe(listener) {
  if (typeof listener !== 'function') {
    return () => {};
  }

  tokenSubscribers.add(listener);
  listener(accessToken);

  return () => {
    tokenSubscribers.delete(listener);
  };
}

function onUnauthorized(listener) {
  if (typeof listener !== 'function') {
    return () => {};
  }

  unauthorizedListeners.add(listener);

  return () => {
    unauthorizedListeners.delete(listener);
  };
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
  const path = getRequestPath(url);
  if (!path) {
    return false;
  }

  return (
    path === AUTH_LOGIN_URL ||
    path === AUTH_REFRESH_URL ||
    path === AUTH_LOGOUT_URL
  );
}

function getRequestPath(url) {
  if (!url) {
    return '';
  }

  try {
    const parsedUrl = new URL(url, 'http://localhost');
    return parsedUrl.pathname || '';
  } catch {
    return '';
  }
}

function isPublicRequest(url) {
  const path = getRequestPath(url);
  if (!path) {
    return false;
  }

  return PUBLIC_ROUTE_PREFIXES.some(prefix => {
    return path === prefix || path.startsWith(`${prefix}/`);
  });
}

async function refreshAccessToken() {
  const { data } = await api.post(
    AUTH_REFRESH_URL,
    null,
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

  const shouldSkipAuthHeader = isPublicRequest(config.url) && !config.attachAuthToken;
  if (!shouldSkipAuthHeader && accessToken) {
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
      isAuthRequest(originalRequest.url) ||
      isPublicRequest(originalRequest.url)
    ) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    if (!refreshPromise) {
      refreshPromise = refreshAccessToken()
        .catch(refreshError => {
          clearTokens({ notifyUnauthorized: true });
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

const tokenManager = {
  getAccessToken,
  setAccessToken: setTokens,
  clearAccessToken: options => clearTokens(options),
  refreshAccessToken,
  subscribe,
  onUnauthorized,
};

export {
  api,
  tokenManager,
  setTokens,
  getAccessToken,
  clearTokens,
  refreshAccessToken,
  subscribe,
  onUnauthorized,
  extractErrorMessage,
  extractAccessToken,
};
