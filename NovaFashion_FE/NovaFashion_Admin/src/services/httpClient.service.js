import axios from "axios";

const httpClient = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const refreshClient = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: { 'Content-Type': 'application/json' },
});

httpClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },

    (error) => Promise.reject(error)
);


let isRefreshing = false;
let pendingRequests = [];

const processPendingRequests = (newToken) => {
    pendingRequests.forEach(({ resolve }) => resolve(newToken));
    pendingRequests = [];
};

httpClient.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status !== 401 || originalRequest._retry) {
            return Promise.reject(error);
        }

        if (isRefreshing) {
            return new Promise((resolve) => {
                pendingRequests.push({ resolve });
            }).then((newToken) => {
                originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
                return httpClient(originalRequest);
            });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
            // import động để tránh circular dependency
            const { authApi } = await import('../features/authentications/authApi');
            const newToken = await authApi.refreshToken();

            processPendingRequests(newToken);
            originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
            return httpClient(originalRequest);

        } catch (refreshError) {
            pendingRequests = [];
            const { authApi } = await import('../features/authentications/authApi');
            authApi.logout();
            globalThis.location.href = '/login';
            return Promise.reject(refreshError);

        } finally {
            isRefreshing = false;
        }
    }
);

export default httpClient;