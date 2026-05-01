import { useState } from "react";
import { productApi } from "../../features/products/productApi";
import useSWRMutation from "swr/mutation";

export const useUpdateProduct = (productId) => {
    const [fieldErrors, setFieldErrors] = useState({});
    const [error, setError] = useState(null);

    const errorMap = {
        product_name: 'name',
        description: 'description',
        details: 'details',
        unit_price: 'basePrice',
        total_quantity: 'totalQuantity',
    };

    const { trigger, isMutating } = useSWRMutation(
        ['updateProduct', productId],
        async (_, { arg: form }) => {
            try {
                return await productApi.update(productId, form);
            } catch (err) {
                const res = err?.response?.data;

                if (res?.errors) {
                    const mapped = {};
                    Object.keys(res.errors).forEach(key => {
                        const field = errorMap[key];
                        if (field) mapped[field] = res.errors[key][0];
                    });
                    setFieldErrors(mapped);
                    setError('Xảy ra lỗi trong quá trình cập nhật. Vui lòng kiểm tra lại thông tin.');
                    throw mapped;
                }

                setError(res?.message || 'Có lỗi xảy ra');
                throw err;
            }
        }
    );

    const clearFieldError = (field) => {
        setFieldErrors(prev => ({ ...prev, [field]: undefined }));
    };

    const resetErrors = () => {
        setFieldErrors({});
        setError(null);
    };

    return {
        updateProduct: trigger,
        isUpdating: isMutating,
        fieldErrors,
        error,
        clearFieldError,
        resetErrors
    };
};