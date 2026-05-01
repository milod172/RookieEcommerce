import httpClient from "../../services/httpClient.service";

export const categoryApi = {
    getAll: async (params) => {
        const res = await httpClient.get('/categories', { params });
        console.log('Fetched categories with params:', params, 'Response:', res.data);
        return res.data;
    },

    getSubCategories: async (parentId) => {
        const res = await httpClient.get(`/categories/${parentId}`);
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
};