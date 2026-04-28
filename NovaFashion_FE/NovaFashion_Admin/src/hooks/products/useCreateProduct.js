import { productApi } from "../../features/products/productApi";
import useSWRMutation from "swr/mutation";

export const useCreateProduct = () => {
    const { trigger, isMutating } = useSWRMutation(
        'createProduct',
        async (_, { arg: { form, images } }) => {
            // 1. tạo product
            const product = await productApi.create(form);
            const productId = product?.id;

            // 2. upload images
            if (images?.length > 0 && productId) {
                const formData = new FormData();

                images.forEach((img) => {
                    formData.append('files', img.file);
                });

                await productApi.uploadImages(productId, formData);
            }

            return productId;
        }
    );

    return {
        createProduct: trigger,
        isCreating: isMutating,
    };
};
