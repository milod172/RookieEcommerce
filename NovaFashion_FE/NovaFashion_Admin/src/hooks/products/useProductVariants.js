import { useEffect, useState } from 'react';
import { variantApi } from '../../features/variants/variantApi';

const SIZE_OPTIONS = ['S', 'M', 'L', 'XL', 'XXL'];

const EMPTY_ADD_DRAFT = {
    size: SIZE_OPTIONS[0],
    stockQuantity: 0,
    unitPrice: 0,
};

const variantErrorMap = {
    size: 'size',
    stock_quantity: 'stockQuantity',
    unit_price: 'unitPrice',
};

const mapVariant = (v) => ({
    id: v.id,
    productId: v.product_id,
    skuVariant: v.variant_sku,
    attrs: v.size,
    stockQuantity: v.stock_quantity,
    unitPrice: v.unit_price,
    status: v.is_deleted ? 'InActive' : 'Active',
});

const parseApiErrors = (err, errorMap) => {
    const res = err?.response?.data;
    if (res?.errors) {
        const mapped = {};
        Object.keys(res.errors).forEach((key) => {
            const field = errorMap[key];
            if (field) mapped[field] = res.errors[key][0];
        });
        console.log('API Errors:', res.errors);
        console.log('Mapped Field Errors:', res);
        return { fieldErrors: mapped, generalError: 'Vui lòng kiểm tra lại thông tin.' };
    }
    return { fieldErrors: {}, generalError: res?.message || 'Có lỗi xảy ra.' };
};

export const useProductVariants = (productId, setIsDirty) => {
    const [variants, setVariants] = useState([]);
    const [isLoadingVariants, setIsLoadingVariants] = useState(false);

    // Add variants
    const [addingRow, setAddingRow] = useState(false);
    const [addDraft, setAddDraft] = useState(EMPTY_ADD_DRAFT);

    // Edit variants
    const [editingVariantId, setEditingVariantId] = useState(null);
    const [variantDraft, setVariantDraft] = useState(null);
    const [editFieldErrors, setEditFieldErrors] = useState({});
    const [editError, setEditError] = useState(null);

    // Handle error
    const [addFieldErrors, setAddFieldErrors] = useState({});
    const [addError, setAddError] = useState(null);


    // Get all
    useEffect(() => {
        if (!productId) return;

        const fetchVariants = async () => {
            setIsLoadingVariants(true);
            try {
                const data = await variantApi.getAll(productId);
                setVariants(data.map(mapVariant));
            } catch (err) {
                console.error('Failed to fetch variants:', err);
            } finally {
                setIsLoadingVariants(false);
            }
        };

        fetchVariants();
    }, [productId]);


    // Add new
    const startAddVariant = () => {
        setAddingRow(true);
        setAddDraft({ ...EMPTY_ADD_DRAFT });
        setAddFieldErrors({});
        setAddError(null);
    };

    const cancelAddVariant = () => {
        setAddingRow(false);
        setAddDraft(EMPTY_ADD_DRAFT);
        setAddFieldErrors({});
        setAddError(null);
    };

    const saveAddVariant = async () => {
        setAddFieldErrors({});
        setAddError(null);

        try {
            const payload = {
                size: addDraft.size,
                stock_quantity: addDraft.stockQuantity,
                unit_price: addDraft.unitPrice,
            }
            const created = await variantApi.create(productId, payload);

            setVariants((prev) => [...prev, mapVariant(created)]);
            setAddingRow(false);
            setAddDraft(EMPTY_ADD_DRAFT);

            if (setIsDirty) setIsDirty(true);

        } catch (err) {

            const { fieldErrors, generalError } = parseApiErrors(err, variantErrorMap);
            setAddFieldErrors(fieldErrors);
            setAddError(generalError);
        }
    };

    const handleAddDraftChange = (e) => {
        const { name, value } = e.target;
        setAddDraft((prev) => ({
            ...prev,
            [name]:
                name === 'stockQuantity' || name === 'unitPrice'
                    ? Number(value) || 0
                    : value,
        }));
    };


    // Update
    const startEditVariant = (variant) => {
        setEditingVariantId(variant.id);
        setVariantDraft({ ...variant });
        setEditFieldErrors({});
        setEditError(null);
    };

    const cancelEditVariant = () => {
        setEditingVariantId(null);
        setVariantDraft(null);
        setEditFieldErrors({});
        setEditError(null);
    };

    const saveEditVariant = async () => {
        if (!variantDraft) return;
        setEditFieldErrors({});
        setEditError(null);

        try {
            const payload = {
                size: variantDraft.attrs,
                stock_quantity: variantDraft.stockQuantity,
                unit_price: variantDraft.unitPrice,
                is_deleted: variantDraft.status === 'InActive',
            }

            const updated = await variantApi.update(productId, variantDraft.id, payload);

            setVariants((prev) =>
                prev.map((v) => (v.id === variantDraft.id ? mapVariant(updated) : v))
            );
            setEditingVariantId(null);
            setVariantDraft(null);
            if (setIsDirty) setIsDirty(true);

        } catch (err) {
            const { fieldErrors, generalError } = parseApiErrors(err, variantErrorMap);
            console.log('Edit Errors:', fieldErrors, generalError);
            setEditFieldErrors(fieldErrors);
            setEditError(generalError);
        }
    };

    const handleVariantDraftChange = (e) => {
        const { name, value } = e.target;
        console.log('CHANGE:', name, value);
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
        isLoadingVariants,

        editingVariantId,
        variantDraft,
        editFieldErrors,
        editError,
        startEditVariant,
        cancelEditVariant,
        saveEditVariant,
        handleVariantDraftChange,

        SIZE_OPTIONS,
        addingRow,
        addDraft,
        addFieldErrors,
        addError,
        startAddVariant,
        cancelAddVariant,
        saveAddVariant,
        handleAddDraftChange,
    };
};