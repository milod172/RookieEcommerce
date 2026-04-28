import useSWR from "swr";
import { productApi } from "../../features/products/productApi";

export const useProducts = (params) => {
    const { data, error, isLoading } = useSWR(
        ['products', params],
        () => productApi.getAll(params),
        {
            keepPreviousData: true,
            revalidateOnFocus: false,
        }
    );

    return {
        products: data?.items || [],
        totalCount: data?.total_count || 0,
        isLoading,
        isError: !!error,
    };
};