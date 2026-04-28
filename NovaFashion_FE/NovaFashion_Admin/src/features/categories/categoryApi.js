import httpClient from "../../services/httpClient.service";

export const categoryApi = {
    getAll: async (params) => {
        const res = await httpClient.get('/categories', { params });
        return res.data;
    },
};