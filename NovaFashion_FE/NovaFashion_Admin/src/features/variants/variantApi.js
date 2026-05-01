import httpClient from "../../services/httpClient.service";

export const variantApi = {
    getAll: async (productId) => {
        const res = await httpClient.get(`/products/${productId}/variants`);
        return res.data;
    },

    create: async (productId, data) => {
        const res = await httpClient.post(`/products/${productId}/variants`, data);
        return res.data;
    },

    update: async (productId, variantId, data) => {
        const res = await httpClient.put(`/products/${productId}/variants/${variantId}`, data);
        return res.data;
    },

};