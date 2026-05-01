import useSWR from 'swr';
import { productApi } from "../../features/products/productApi";
import { useMemo } from 'react';

export const useProductDetails = (id) => {
    const { data, error, isLoading, mutate } = useSWR(
        id ? ['product', id] : null,
        () => productApi.getById(id),
        {
            revalidateOnFocus: false,
        }
    );

    const product = useMemo(() => {
        if (!data) return null;
        return {
            id: data.id,
            sku: data.sku,
            name: data.product_name,
            description: data.description,
            details: data.details,
            totalQuantity: data.total_quantity,
            totalSell: data.total_sell,
            basePrice: data.unit_price,
            categoryId: data.category_id,
            categoryName: data.category_name,
            images: data.images || [],
            variants: data.variants,
            isDeleted: data.is_deleted,
        };
    }, [data]);

    return {
        product,
        isLoading,
        isError: !!error,
        error,
        mutateProduct: mutate,
    };
};