import httpClient, { refreshClient } from "../../services/httpClient.service";

export const authApi = {
    login: async (credentials) => {
        const res = await httpClient.post('/auth/login', credentials);
        localStorage.setItem('access_token', res.data.access_token);
        localStorage.setItem('refresh_token', res.data.refresh_token);
        localStorage.setItem('user_id', res.data.user_id);
        return res.data;
    },

    register: async (userData) => {
        const res = await httpClient.post('/auth/register', userData);
        return res.data;
    },

    logout: () => {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user_id');
    },

    refreshToken: async () => {
        const userId = localStorage.getItem('user_id');
        const refreshToken = localStorage.getItem('refresh_token');

        //Debug
        const oldAccessToken = localStorage.getItem('access_token');
        const oldRefreshToken = localStorage.getItem('refresh_token');
        const oldAccessPayload = JSON.parse(atob(oldAccessToken.split('.')[1]));
        const oldRefreshPayload = JSON.parse(atob(oldRefreshToken.split('.')[1]));
        console.log('🔴 Access token hết hạn — bắt đầu refresh...');
        console.group('📦 Token cũ');
        console.log('Access token  :', oldAccessToken);
        console.log('Access hết hạn:', new Date(oldAccessPayload.exp * 1000).toLocaleTimeString());
        console.log('Refresh token :', oldRefreshToken);
        console.log('Refresh hết hạn:', new Date(oldRefreshPayload.exp * 1000).toLocaleTimeString());
        console.groupEnd();
        //End Debug


        const res = await refreshClient.post('/auth/refresh-token', {
            user_id: userId,
            refresh_token: refreshToken,
        });

        //Debug
        const newAccessToken = res.data.access_token;
        const newRefreshToken = res.data.refresh_token;
        const newAccessPayload = JSON.parse(atob(newAccessToken.split('.')[1]));
        const newRefreshPayload = JSON.parse(atob(newRefreshToken.split('.')[1]));
        console.log('✅ Refresh thành công!');
        console.group('📦 Token mới');
        console.log('Access token  :', newAccessToken);
        console.log('Access hết hạn:', new Date(newAccessPayload.exp * 1000).toLocaleTimeString());
        console.log('Refresh token :', newRefreshToken);
        console.log('Refresh hết hạn:', new Date(newRefreshPayload.exp * 1000).toLocaleTimeString());
        console.groupEnd();
        //End Debug

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