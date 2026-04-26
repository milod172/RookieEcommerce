import { productApi } from "../../features/products/productApi";


export const useCreateProduct = () => {
    const createProduct = async (form, images) => {
        try {

            const res = await productApi.create({
                product_name: form.name,
                description: form.description,
                unit_price: Number(form.basePrice),
                details: form.details,
                total_quantity: Number(form.totalQuantity),
                category_id: form.categoryId || null,
            });

            const productId = res.data?.id;

            if (images?.length > 0 && productId) {

                const formData = new FormData();

                images.forEach((img) => {
                    formData.append("files", img.file);
                });

                await productApi.uploadImages(productId, formData);
            }

            return productId;

        } catch (err) {
            console.error(err);
            throw err;
        }
    };

    return { createProduct };

};
