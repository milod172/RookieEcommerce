import useSWR from 'swr';
import { categoryApi } from '../../features/categories/categoryApi';

export const useSubCategories = (parentId) => {
    const { data, error, isLoading } = useSWR(
        parentId ? ['subcategories', parentId] : null,
        () => categoryApi.getSubCategories(parentId),
        {
            revalidateOnFocus: false,
        }
    );

    return {
        subCategories: data || [],
        isLoading,
        isError: !!error,
    };
};