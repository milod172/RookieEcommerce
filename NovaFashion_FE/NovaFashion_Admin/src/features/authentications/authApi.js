import httpClient, { refreshClient } from "../../services/httpClient.service";

export const authApi = {
    login: async (credentials) => {
        const res = await httpClient.post('/auth/login', credentials);
        localStorage.setItem('access_token', res.data.access_token);
        localStorage.setItem('refresh_token', res.data.refresh_token);
        // localStorage.setItem('user_id', res.data.user_id);
        return res.data;
    },

    register: async (userData) => {
        const res = await httpClient.post('/auth/register', userData);
        return res.data;
    },

    logout: () => {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        // localStorage.removeItem('user_id');
    },

    refreshToken: async () => {
        const userId = authApi.getPayload()?.sub;
        const refreshToken = localStorage.getItem('refresh_token');

        const res = await refreshClient.post('/auth/refresh-token', {
            user_id: userId,
            refresh_token: refreshToken,
        });

        localStorage.setItem('access_token', res.data.access_token);
        localStorage.setItem('refresh_token', res.data.refresh_token);
        return res.data.access_token;
    },

    getPayload: () => {
        const token = localStorage.getItem('access_token');
        if (!token) return null;
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch {
            return null;
        }
    },

    isAuthenticated: () => {
        const payload = authApi.getPayload();
        if (!payload) return false;
        return payload.exp * 1000 > Date.now();
    },

    getRole: () => {
        const payload = authApi.getPayload();
        return payload?.role ?? null;
    },

    isAdmin: () => {
        return authApi.getRole() === 'Admin';
    }
}