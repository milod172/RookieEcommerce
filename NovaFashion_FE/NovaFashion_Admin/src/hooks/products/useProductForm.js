import { useState, useMemo, useEffect } from 'react';
import { CATEGORIES } from '../../constants/productConstants';

export const useProductForm = (product) => {
    const [form, setForm] = useState({});
    const [isDirty, setIsDirty] = useState(false);

    // Đồng bộ dữ liệu từ API khi product thay đổi
    useEffect(() => {
        if (!product) return;

        setForm({
            id: product.id,
            name: product.name || '',
            description: product.description || '',
            details: product.details || '',
            basePrice: product.basePrice || 0,
            totalQuantity: product.totalQuantity || 0,
            totalSell: product.totalSell || 0,
            sku: product.sku || '',
            categoryId: product.categoryId || '',
            subCategoryId: product.subCategoryId || '', // nếu API có
            status: product.status || 'active',
        });

        setIsDirty(false);
    }, [product]);

    const subCategories = useMemo(() => {
        const cat = CATEGORIES.find((c) => c.id === form.categoryId);
        return cat?.subCategories ?? [];
    }, [form.categoryId]);

    const markDirty = () => setIsDirty(true);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        markDirty();
    };

    const handleCategoryChange = (e) => {
        setForm((prev) => ({
            ...prev,
            categoryId: e.target.value,
            subCategoryId: '',
        }));
        markDirty();
    };

    const handleDiscard = () => {
        if (product) {
            setForm({
                id: product.id,
                name: product.name || '',
                description: product.description || '',
                details: product.details || '',
                basePrice: product.basePrice || 0,
                totalQuantity: product.totalQuantity || 0,
                totalSell: product.totalSell || 0,
                sku: product.sku || '',
                categoryId: product.categoryId || '',
                subCategoryId: product.subCategoryId || '',
                status: product.status || 'active',
            });
        }
        setIsDirty(false);
    };

    return {
        form,
        setForm,
        isDirty,
        setIsDirty,
        subCategories,
        handleChange,
        handleCategoryChange,
        handleDiscard,
    };
};