import httpClient from "../../services/httpClient.service";

export const orderApi = {

    getAll: async (params) => {
        const res = await httpClient.get('/orders', { params });
        return res.data;
    },

    getById: async (id) => {
        const res = await httpClient.get(`/orders/${id}`);
        return res.data;
    },

}