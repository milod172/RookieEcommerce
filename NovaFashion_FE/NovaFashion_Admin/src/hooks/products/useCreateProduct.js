import { useState } from "react";
import { productApi } from "../../features/products/productApi";
import useSWRMutation from "swr/mutation";

export const useCreateProduct = () => {
    const [fieldErrors, setFieldErrors] = useState({});
    const [error, setError] = useState(null);

    const errorMap = {
        product_name: 'name',
        description: 'description',
        details: 'details',
        unit_price: 'basePrice',
        total_quantity: 'totalQuantity',
        category_id: 'categoryId'
    };

    const { trigger, isMutating } = useSWRMutation(
        'createProduct',
        async (_, { arg: { form, images } }) => {
            try {

                const product = await productApi.create(form);
                const productId = product?.id;


                if (images?.length > 0 && productId) {
                    const formData = new FormData();

                    images.forEach((img) => {
                        formData.append('files', img.file);
                    });

                    await productApi.uploadImages(productId, formData);
                }

                return productId;

            } catch (err) {
                const res = err?.response?.data;


                if (res?.errors) {
                    const mapped = {};

                    Object.keys(res.errors).forEach(key => {
                        const field = errorMap[key];
                        if (field) {
                            mapped[field] = res.errors[key][0];
                        }
                    });

                    setFieldErrors(mapped);
                    setError('Xảy ra lỗi trong quá trình tạo sản phẩm. Vui lòng kiểm tra lại thông tin.');
                    throw mapped;
                }


                setError(res?.message || 'Có lỗi xảy ra');
                throw err;
            }
        }
    );


    const clearFieldError = (field) => {
        setFieldErrors(prev => ({
            ...prev,
            [field]: undefined
        }));
    };

    const resetErrors = () => {
        setFieldErrors({});
        setError(null);
    };

    return {
        createProduct: trigger,
        isCreating: isMutating,
        fieldErrors,
        error,
        clearFieldError,
        resetErrors
    };
};
