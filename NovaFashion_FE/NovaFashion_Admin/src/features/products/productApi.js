import httpClient from "@/services/httpClient";

export const productApi = {
    getAll: (params) =>
        httpClient.get("/products", { params }),

    getById: (id) =>
        httpClient.get(`/products/${id}`),

    create: (data) =>
        httpClient.post("/products", data),
};