import httpClient from "../../services/httpClient.service";

export const productApi = {
    getAll: async (params) => {
        const res = await httpClient.get('/products', { params });
        return res.data;
    },

    getById: async (id) => {
        const res = await httpClient.get(`/products/${id}`);
        return res.data;
    },

    create: async (data) => {
        const res = await httpClient.post('/products', data);
        return res.data;
    },

    uploadImages: async (productId, formData) => {
        const res = await httpClient.post(
            `/products/${productId}/images`,
            formData,
            {
                headers: {
                    'Content-Type': undefined
                }
            }
        );
        return res.data;
    },
};