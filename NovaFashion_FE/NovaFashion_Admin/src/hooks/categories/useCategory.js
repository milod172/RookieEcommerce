import useSWR, { mutate } from 'swr';
import useSWRMutation from "swr/mutation";
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

const PICKER_KEY = 'categories/picker';
const CREATE_KEY = 'categories/create';
const UPDATE_KEY = 'categories/update';

export const useCategoriesPicker = () => {
    const { data, error, isLoading } = useSWR(
        PICKER_KEY,
        () => categoryApi.getPicker(),
        {
            revalidateOnFocus: false,
        }
    );

    return {
        pickerItems: data || [],
        isLoading,
        isError: !!error,
    };
};

export const useCreateCategory = () => {
    const { trigger, isMutating, error } = useSWRMutation(
        CREATE_KEY,
        async (_key, { arg }) => {
            return categoryApi.createCategory(arg);
        },
        {
            onSuccess: () => {
                mutate(PICKER_KEY);
                mutate((key) => Array.isArray(key) && key[0] === 'categories');
            },
        }
    );

    return {
        createCategory: trigger,
        isCreating: isMutating,
        createError: error,
    };
};

export const useUpdateCategory = () => {
    const { trigger, isMutating, error } = useSWRMutation(
        UPDATE_KEY,
        async (_key, { arg }) => {
            return categoryApi.updateCategory(arg.id, arg);
        },
        {
            onSuccess: () => {
                mutate(PICKER_KEY);
                mutate((key) => Array.isArray(key) && key[0] === 'categories');
                mutate((key) => Array.isArray(key) && key[0] === 'subCategories');
            },
        }
    );

    return {
        updateCategory: trigger,
        isUpdating: isMutating,
        updateError: error,
    };
};