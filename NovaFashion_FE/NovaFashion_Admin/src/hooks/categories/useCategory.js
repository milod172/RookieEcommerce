import useSWR from 'swr';
import { categoryApi } from '../../features/categories/categoryApi';

export const useCategories = (params) => {
    const { data, error, isLoading } = useSWR(
        ['categories', params],
        () => categoryApi.getAll(params),
        {
            keepPreviousData: true,
            revalidateOnFocus: false,
        }
    );

    return {
        categories: data?.items || [],
        totalCount: data?.total_count || 0,
        totalPages: data?.total_pages || 1,
        isLoading,
        isError: !!error,
    };
};