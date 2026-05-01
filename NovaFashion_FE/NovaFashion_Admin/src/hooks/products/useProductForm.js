import { useState } from 'react';


export const useProductForm = (product, clearFieldError) => {
    const [form, setForm] = useState(null);
    const [isDirty, setIsDirty] = useState(false);

    const mapProductToForm = (product) => {
        return {
            id: product.id,
            name: product.name || '',
            description: product.description || '',
            details: product.details || '',
            basePrice: product.basePrice || 0,
            totalQuantity: product.totalQuantity || 0,
            totalSell: product.totalSell || 0,
            sku: product.sku || '',
            status: product.status || 'active',
            isDeleted: product.isDeleted || false,
        };
    };

    const currentForm = form ?? (product ? mapProductToForm(product) : {});

    const handleChange = (e) => {
        const { name, value } = e.target;

        setForm((prev) => {
            const base = prev ?? mapProductToForm(product);

            return {
                ...base,
                [name]: value,
            };
        });
        clearFieldError(name);
        setIsDirty(true)
    };

    const handleDiscard = () => {
        setForm(null);
        setIsDirty(false);
    };

    return {
        form: currentForm,
        isDirty,
        setForm,
        setIsDirty,
        handleChange,
        handleDiscard,
    };
};