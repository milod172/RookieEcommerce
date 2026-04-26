import httpClient from "../../services/httpClient.service";


export const productApi = {
    getAll: (params) =>
        httpClient.get("/products", { params }),

    getById: (id) =>
        httpClient.get(`/products/${id}`),

    create: (data) =>
        httpClient.post("/products", data),

    uploadImages: (productId, formData) =>
        httpClient.post(`/products/${productId}/images`, formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        }),
};