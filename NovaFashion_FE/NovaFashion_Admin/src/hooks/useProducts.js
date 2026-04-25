import useSWR from "swr";
import httpClient from "../services/httpClient.service.js";

const fetcher = (url) => httpClient.get(url).then(res => res.data);

export const useProducts = (params) => {
    const query = new URLSearchParams(params).toString();

    const { data, error, isLoading } = useSWR(
        `/products?${query}`,
        fetcher,
        {
            keepPreviousData: true,
            revalidateOnFocus: false, //Switch Tab
        }
    );

    return {
        products: data?.items || [],
        totalCount: data?.total_count || 0,
        isLoading,
        isError: error,
    };
};