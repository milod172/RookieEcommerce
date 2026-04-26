import axios from "axios";

const httpClient = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

httpClient.interceptors.request.use(
    (config) => {

        return config;
    },
    (error) => Promise.reject(error)
);

export default httpClient;