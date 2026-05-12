/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useProductDetails } from '../hooks/products/useProductDetails';
import { useProductForm } from '../hooks/products/useProductForm';
import { useProductImages } from '../hooks/products/useProductImages';
import { useProductVariants } from '../hooks/products/useProductVariants';
import ProductHeader from '../features/products/components/ProductHeader';
import ProductGeneralInfo from '../features/products/components/ProductGeneralInfo';
import ProductImages from '../features/products/components/ProductImages';
import ProductVariants from '../features/products/components/ProductVariants';
import ProductStats from '../features/products/components/ProductStats';
import ProductPricing from '../features/products/components/ProductPricing';
import ProductCategorization from '../features/products/components/ProductCategorization';
import ProductStatus from '../features/products/components/ProductStatus';
import { useCategoryTree } from '../hooks/products/useCategoryTree';
import { useUpdateProduct } from '../hooks/products/useUpdateProduct';

const ProductDetails = () => {
    const { id } = useParams();
    const { product, isLoading, isError, mutateProduct } = useProductDetails(id);
    const { updateProduct, isUpdating, fieldErrors, error, clearFieldError, resetErrors } = useUpdateProduct(id);
    const formLogic = useProductForm(product, clearFieldError);
    const imageLogic = useProductImages(id, mutateProduct);

    //Categorization
    const [isChangingCategory, setIsChangingCategory] = useState(false);
    const [selectedPath, setSelectedPath] = useState([]);
    const [categoryDirty, setCategoryDirty] = useState(false);
    const { categoryDropdowns,
        finalCategoryId,
        breadcrumbPath,
        handleCategorySelect: _handleCategorySelect,
        isLoading: categoriesLoading,
        isError: categoriesError,
    } = useCategoryTree(selectedPath, setSelectedPath);

    const handleCategorySelect = (level, value) => {
        _handleCategorySelect(level, value);
        setCategoryDirty(true);
    };

    const variantLogic = useProductVariants(id, formLogic.setIsDirty);

    const isAnythingDirty = formLogic.isDirty || categoryDirty;

    const handleSave = async (e) => {
        e.preventDefault();
        resetErrors();

        try {
            const formPayload = {
                product_name: formLogic.form.name,
                description: formLogic.form.description,
                unit_price: Number(formLogic.form.basePrice),
                details: formLogic.form.details,
                total_quantity: Number(formLogic.form.totalQuantity),
                category_id: categoryDirty
                    ? finalCategoryId
                    : product.categoryId,
                is_deleted: formLogic.form.isDeleted,
            };

            const updated = await updateProduct(formPayload);
            await mutateProduct(updated, { revalidate: false }); //updated cache bằng updated, không gọi lại api

            formLogic.setForm(null);
            setSelectedPath([]);
            formLogic.setIsDirty(false);
            setCategoryDirty(false);
            setIsChangingCategory(false);

        } catch (err) {
            console.error('Update failed:', err);
            console.error('Response:', err?.response?.data);
        }
    };

    const handleDiscard = () => {
        resetErrors();
        formLogic.handleDiscard();
        setCategoryDirty(false);
        setSelectedPath([]);
        setIsChangingCategory(false);
    };


    useEffect(() => {
        if (!product) return;

        const mappedImages = (product.images || []).map((img, index) => ({
            id: img.id,
            url: img.image_url,
            name: img.alt_text || `image-${index}`,
            isPrimary: img.is_primary || false,
            sortOrder: img.sort_order || index,
            isNew: false,
        }));

        mappedImages.sort((a, b) => a.sortOrder - b.sortOrder);

        const primaryIdx = mappedImages.findIndex((img) => img.isPrimary);
        if (primaryIdx > 0) {
            const primary = mappedImages.splice(primaryIdx, 1)[0];
            mappedImages.unshift(primary);
        }

        imageLogic.setImages(mappedImages);

    }, [product?.id]);


    if (isLoading) return <p>Loading...</p>;
    if (isError) return <p>Error loading product</p>;
    if (!product) return <p>No product found</p>;

    return (
        <div className="container-fluid">
            <form onSubmit={handleSave}>
                <ProductHeader
                    form={formLogic.form}
                    isDirty={isAnythingDirty}
                    handleDiscard={handleDiscard}
                    isUpdating={isUpdating} />

                {error && (
                    <div className="alert alert-danger d-flex align-items-center gap-2 mb-3" role="alert">
                        <i className="bi bi-exclamation-triangle-fill"></i>
                        <span>{error}</span>
                    </div>
                )}

                <div className="row g-3">
                    {/* LEFT — Main info */}
                    <div className="col-lg-8">
                        <ProductGeneralInfo form={formLogic.form} handleChange={formLogic.handleChange} fieldErrors={fieldErrors} />
                        <ProductImages
                            images={imageLogic.images}
                            handleSetCover={imageLogic.handleSetCover}
                            handleRemoveImage={imageLogic.handleRemoveImage}
                            handleDrop={imageLogic.handleDrop}
                            handleDragOver={imageLogic.handleDragOver}
                            fileInputRef={imageLogic.fileInputRef}
                            handleFiles={imageLogic.handleFiles}
                            isUploading={imageLogic.isUploading}
                            isDeleting={imageLogic.isDeleting}
                        />
                        <ProductVariants
                            isLoadingVariants={variantLogic.isLoadingVariants}
                            variants={variantLogic.variants}
                            editingVariantId={variantLogic.editingVariantId}
                            variantDraft={variantLogic.variantDraft}
                            startEditVariant={variantLogic.startEditVariant}
                            cancelEditVariant={variantLogic.cancelEditVariant}
                            saveEditVariant={variantLogic.saveEditVariant}
                            handleVariantDraftChange={variantLogic.handleVariantDraftChange}
                            addingRow={variantLogic.addingRow}
                            addDraft={variantLogic.addDraft}
                            startAddVariant={variantLogic.startAddVariant}
                            cancelAddVariant={variantLogic.cancelAddVariant}
                            saveAddVariant={variantLogic.saveAddVariant}
                            handleAddDraftChange={variantLogic.handleAddDraftChange}
                            SIZE_OPTIONS={variantLogic.SIZE_OPTIONS}
                            addFieldErrors={variantLogic.addFieldErrors}
                            addError={variantLogic.addError}
                            editFieldErrors={variantLogic.editFieldErrors}
                            editError={variantLogic.editError}
                        />
                    </div>

                    {/* RIGHT — Side panels */}
                    <div className="col-lg-4">
                        <ProductStats form={formLogic.form} />
                        <ProductPricing form={formLogic.form} handleChange={formLogic.handleChange} fieldErrors={fieldErrors} />
                        <ProductCategorization
                            currentCategoryName={product.categoryName}
                            categoryDropdowns={categoryDropdowns}
                            breadcrumbPath={breadcrumbPath}
                            handleCategorySelect={handleCategorySelect}
                            isChanging={isChangingCategory}
                            setIsChanging={setIsChangingCategory}
                            isLoading={categoriesLoading}
                            isError={categoriesError}
                        />
                        <ProductStatus form={formLogic.form} handleChange={formLogic.handleChange} />
                    </div>
                </div>
            </form>
        </div>
    );
};

export default ProductDetails;