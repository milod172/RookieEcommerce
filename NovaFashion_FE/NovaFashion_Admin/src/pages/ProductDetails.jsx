/* eslint-disable react-hooks/set-state-in-effect */
import { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useProductDetails } from '../hooks/products/useProductDetails';
import { useProductForm } from '../hooks/products/useProductForm';
import { useProductImages } from '../hooks/products/useProductImages';
import { useProductVariants } from '../hooks/products/useProductVariants';
import ProductHeader from '../components/products/ProductHeader';
import ProductGeneralInfo from '../components/products/ProductGeneralInfo';
import ProductImages from '../components/products/ProductImages';
import ProductVariants from '../components/products/ProductVariants';
import ProductStats from '../components/products/ProductStats';
import ProductPricing from '../components/products/ProductPricing';
import ProductCategorization from '../components/products/ProductCategorization';
import ProductStatus from '../components/products/ProductStatus';

const ProductDetails = () => {
    const { id } = useParams();
    const { product, isLoading, isError } = useProductDetails(id);
    const formLogic = useProductForm(product);
    const imageLogic = useProductImages();
    const variantLogic = useProductVariants(formLogic.setIsDirty);

    const handleSave = (e) => {
        e.preventDefault();
        const payload = { ...formLogic.form, images: imageLogic.images, variants: variantLogic.variants };
        console.log('Save product payload:', payload);

        // TODO: Gọi API update ở đây (FormData nếu có file mới)
        formLogic.setIsDirty(false);
    };

    useEffect(() => {
        if (!product) return
        const mappedImages = (product.images || []).map((img, index) => ({
            id: img.image_url || `img-${index}`,
            url: img.image_url,
            name: img.alt_text || `image-${index}`,
            isPrimary: img.is_primary || false,
            sortOrder: img.sort_order || index,

        }));


        mappedImages.sort((a, b) => a.sortOrder - b.sortOrder);
        const primaryIdx = mappedImages.findIndex((img) => img.isPrimary);
        if (primaryIdx > 0) {
            const primary = mappedImages.splice(primaryIdx, 1)[0];
            mappedImages.unshift(primary);
        }

        imageLogic.setImages(mappedImages);
        variantLogic.setVariants(product.variants || []);
        variantLogic.setEditingVariantId(null);
        variantLogic.setVariantDraft(null);
    }, [product]);


    if (isLoading) return <p>Loading...</p>;
    if (isError) return <p>Error loading product</p>;
    if (!product) return <p>No product found</p>;

    return (
        <div className="container-fluid">
            <form onSubmit={handleSave}>
                <ProductHeader form={formLogic.form} isDirty={formLogic.isDirty} handleDiscard={formLogic.handleDiscard} />
                <div className="row g-3">
                    {/* LEFT — Main info */}
                    <div className="col-lg-8">
                        <ProductGeneralInfo form={formLogic.form} handleChange={formLogic.handleChange} />
                        <ProductImages
                            images={imageLogic.images}
                            handleSetCover={imageLogic.handleSetCover}
                            handleRemoveImage={imageLogic.handleRemoveImage}
                            handleDrop={imageLogic.handleDrop}
                            handleDragOver={imageLogic.handleDragOver}
                            fileInputRef={imageLogic.fileInputRef}
                            handleFiles={imageLogic.handleFiles}
                        />
                        <ProductVariants
                            variants={variantLogic.variants}
                            editingVariantId={variantLogic.editingVariantId}
                            variantDraft={variantLogic.variantDraft}
                            startEditVariant={variantLogic.startEditVariant}
                            cancelEditVariant={variantLogic.cancelEditVariant}
                            saveEditVariant={variantLogic.saveEditVariant}
                            handleVariantDraftChange={variantLogic.handleVariantDraftChange}
                        />
                    </div>

                    {/* RIGHT — Side panels */}
                    <div className="col-lg-4">
                        <ProductStats form={formLogic.form} />
                        <ProductPricing form={formLogic.form} handleChange={formLogic.handleChange} />
                        <ProductCategorization
                            form={formLogic.form}
                            subCategories={formLogic.subCategories}
                            handleChange={formLogic.handleChange}
                            handleCategoryChange={formLogic.handleCategoryChange}
                        />
                        <ProductStatus form={formLogic.form} handleChange={formLogic.handleChange} />
                    </div>
                </div>
            </form>
        </div>
    );
};

export default ProductDetails;