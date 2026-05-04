import httpClient from "../../services/httpClient.service";

export const userApi = {

    getAll: async (params) => {
        const res = await httpClient.get('/users', { params });
        return res.data;
    },

}

