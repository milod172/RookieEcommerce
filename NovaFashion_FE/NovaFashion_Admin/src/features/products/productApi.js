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

    update: async (id, data) => {
        const res = await httpClient.put(`/products/${id}`, data);
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

    deleteImage: async (productId, imageId) => {
        const res = await httpClient.delete(
            `/products/${productId}/images/${imageId}`
        );
        return res.data;
    },


};