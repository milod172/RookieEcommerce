import useSWR from "swr";
import { userApi } from "../../features/users/userApi";

export const useUsers = (params) => {
    const { data, error, isLoading } = useSWR(
        ['users', params],
        () => userApi.getAll(params),
        {
            keepPreviousData: true,
            revalidateOnFocus: false,
        }
    );

    return {
        users: data?.items || [],
        totalCount: data?.total_count || 0,
        isLoading,
        isError: !!error,
    };
}