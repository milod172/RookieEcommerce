import useSWR from "swr";
import { orderApi } from "../../features/orders/orderApi";

export const useOrders = (params) => {
    const { data, error, isLoading } = useSWR(
        ['orders', params],
        () => orderApi.getAll(params),
        {
            keepPreviousData: true,
            revalidateOnFocus: false,
        }
    );

    return {
        orders: data?.items || [],
        totalCount: data?.total_count || 0,
        isLoading,
        isError: !!error,
    };
};

export const useOrderDetails = (id) => {
    const { data, error, isLoading } = useSWR(
        id ? ['orders', id] : null,  // null tức là không fetch khi chưa có id
        () => orderApi.getById(id),
        {
            revalidateOnFocus: false,
        }
    );

    return {
        order: data || null,
        isLoading,
        isError: !!error,
    };
};