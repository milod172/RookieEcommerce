import httpClient from "../../services/httpClient.service";

export const categoryApi = {
    getAll: async (params) => {
        const res = await httpClient.get('/categories', { params });
        return res.data;
    },

    getPicker: async () => {
        const res = await httpClient.get('/categories/picker');
        return res.data;
    },

    createCategory: async (data) => {
        const res = await httpClient.post('/categories', data);
        return res.data;
    },

    getCategoryById: async (id) => {
        const res = await httpClient.get(`/categories/${id}`);
        return res.data;
    },

    updateCategory: async (id, data) => {
        const res = await httpClient.put(`/categories/${id}`, data);
        return res.data;
    },

};