import { useState } from 'react';

export const useProductVariants = (setIsDirty) => {
    const [variants, setVariants] = useState([]);
    const [editingVariantId, setEditingVariantId] = useState(null);
    const [variantDraft, setVariantDraft] = useState(null);

    const startEditVariant = (variant) => {
        setEditingVariantId(variant.id);
        setVariantDraft({ ...variant });
    };

    const cancelEditVariant = () => {
        setEditingVariantId(null);
        setVariantDraft(null);
    };

    const saveEditVariant = () => {
        if (!variantDraft) return;
        setVariants((prev) =>
            prev.map((v) => (v.id === variantDraft.id ? { ...variantDraft } : v))
        );
        setEditingVariantId(null);
        setVariantDraft(null);
        if (setIsDirty) setIsDirty(true);
    };

    const handleVariantDraftChange = (e) => {
        const { name, value } = e.target;
        setVariantDraft((prev) => ({
            ...prev,
            [name]:
                name === 'stockQuantity' || name === 'unitPrice'
                    ? Number(value) || 0
                    : value,
        }));
    };

    return {
        variants,
        setVariants,
        editingVariantId,
        setEditingVariantId,
        variantDraft,
        setVariantDraft,
        startEditVariant,
        cancelEditVariant,
        saveEditVariant,
        handleVariantDraftChange,
    };
};