/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { useState, useEffect, useMemo } from 'react';
import { useCategoriesPicker, useCreateCategory, useUpdateCategory } from '../../../hooks/categories/useCategory';


function flattenPickerItems(items = []) {
    const byParent = new Map();
    items.forEach((c) => {
        const key = c.parent_category_id ?? null;
        if (!byParent.has(key)) byParent.set(key, []);
        byParent.get(key).push(c);
    });

    const result = [];
    const walk = (parentId, level) => {
        const children = byParent.get(parentId) ?? [];
        for (const node of children) {
            result.push({ ...node, level });
            walk(node.id, level + 1);
        }
    };
    walk(null, 0);
    return result;
}

const INDENT = (level) => (level > 0 ? `${'—'.repeat(level)} ` : '');

const EMPTY = { name: '', description: '', parentId: '' };

const CategoryFormModal = ({
    mode = 'create',
    show,
    onHide,
    onSuccess,
    initialValues = EMPTY,
    title = mode === 'create' ? 'Tạo danh mục mới' : 'Cập nhật danh mục',
}) => {

    const [name, setName] = useState(initialValues.name ?? '');
    const [description, setDescription] = useState(initialValues.description ?? '');
    const [parentId, setParentId] = useState(initialValues.parentId ?? '');
    const [formError, setFormError] = useState({});
    const [isActive, setIsActive] = useState(initialValues.isActive ?? true);

    const isCreate = mode === 'create';
    const isUpdate = mode === 'update';

    // Reset whenever the modal opens
    useEffect(() => {
        if (show) {
            setName(initialValues.name ?? '');
            setDescription(initialValues.description ?? '');
            setParentId(initialValues.parentId ?? '');
            setIsActive(initialValues.isActive ?? true);
            setFormError({});
        }
    }, [show]);


    const { pickerItems, isLoading: pickerLoading, isError: pickerError } = useCategoriesPicker();
    const flatItems = useMemo(() => flattenPickerItems(pickerItems), [pickerItems]);

    const { createCategory, isCreating, createError } = useCreateCategory();
    const { updateCategory, isUpdating, updateError } = useUpdateCategory();

    const isSubmitting = isCreating || isUpdating;

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFormError({});

        try {
            if (isCreate) {
                await createCategory({
                    category_name: name.trim(),
                    description: description.trim(),
                    parent_category_id: parentId || null,
                });
            } else {
                const payload = {
                    id: initialValues.id,
                    category_name: name.trim(),
                    description: description.trim(),
                    parent_category_id: parentId || null,
                    is_deleted: !isActive,
                };
                console.log('Update payload:', payload);
                await updateCategory(payload);
            }

            onSuccess?.();
            onHide?.();
        } catch (err) {
            const errors = err?.response?.data?.errors;
            if (errors) {

                setFormError({
                    name: errors?.category_name?.[0],
                    description: errors?.description?.[0],
                    status: errors?.general_errors?.[0],
                });
            } else {
                setFormError({
                    general: err?.response?.data?.message || `Có lỗi xảy ra khi ${isCreate ? 'tạo' : 'cập nhật'} danh mục.`,
                });
            }
        }
    };

    if (!show) return null;

    return (
        <>
            {/* BACKDROP */}
            <div className="modal-backdrop fade show"></div>

            {/* MODAL */}
            <div className="modal fade show d-block" tabIndex="-1">
                <div className="modal-dialog modal-dialog-centered">
                    <div className="modal-content">

                        {/* HEADER */}
                        <div className="modal-header border-bottom-0 pb-0">
                            <h5 className="modal-title fw-semibold">{title}</h5>
                            <button
                                type="button"
                                className="btn-close"
                                onClick={onHide}
                                disabled={isSubmitting}
                            />
                        </div>

                        {/* BODY */}
                        <form onSubmit={handleSubmit}>
                            <div className="modal-body pt-2">

                                {/* GENERAL ERROR */}
                                {formError.general && (
                                    <div className="alert alert-danger py-2 px-3 small">{formError.general}</div>
                                )}

                                {/* NAME */}
                                <div className="mb-3">
                                    <label htmlFor='category-name' className="form-label small fw-medium">
                                        Tên danh mục <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        type="text"
                                        id='category-name'
                                        className={`form-control ${formError.name ? 'is-invalid' : ''}`}
                                        value={name}
                                        onChange={(e) => setName(e.target.value)}
                                        disabled={isSubmitting}
                                        autoFocus
                                        required
                                    />
                                    {formError.name && <div className="invalid-feedback">{formError.name}</div>}
                                </div>

                                {/* DESCRIPTION */}
                                <div className="mb-3">
                                    <label htmlFor='category-description' className="form-label small fw-medium">Mô tả</label>
                                    <textarea
                                        id='category-description'
                                        className={`form-control ${formError.description ? 'is-invalid' : ''}`}
                                        rows={3}
                                        value={description}
                                        onChange={(e) => setDescription(e.target.value)}
                                        disabled={isSubmitting}
                                        style={{ resize: 'none' }}
                                    />
                                    {formError.description && <div className="invalid-feedback">{formError.description}</div>}
                                </div>

                                {/* PARENT */}
                                {isCreate && (
                                    <div>
                                        <label htmlFor='parent-category' className="form-label small fw-medium">Danh mục cha</label>

                                        {pickerError ? (
                                            <div className="alert alert-warning py-2 px-3 small">
                                                Không thể tải danh sách danh mục.
                                            </div>
                                        ) : (
                                            <div className="position-relative">
                                                <select
                                                    id='parent-category'
                                                    className="form-select"
                                                    value={parentId}
                                                    onChange={(e) => setParentId(e.target.value)}
                                                    disabled={isSubmitting || pickerLoading}
                                                >
                                                    <option value="">
                                                        — Không có (danh mục gốc) —
                                                    </option>
                                                    {flatItems.map((c) => (
                                                        <option key={c.id} value={c.id}>
                                                            {`${INDENT(c.level)}${c.category_name}`}
                                                        </option>
                                                    ))}
                                                </select>

                                                {pickerLoading && (
                                                    <div className="position-absolute top-50 end-0 translate-middle-y me-3">
                                                        <div className="spinner-border spinner-border-sm" />
                                                    </div>
                                                )}
                                            </div>
                                        )}

                                        <div className="form-text">
                                            Dấu "—" thể hiện cấp độ danh mục.
                                        </div>
                                    </div>
                                )}

                                {isUpdate && (
                                    <div className="mb-3">
                                        <label htmlFor='category-status' className="form-label small fw-medium">Trạng thái</label>
                                        <select
                                            id='category-status'
                                            className={`form-select ${formError.status ? 'is-invalid' : ''}`}
                                            value={isActive ? 'active' : 'inactive'}
                                            onChange={(e) => setIsActive(e.target.value === 'active')}
                                            disabled={isSubmitting}
                                        >
                                            <option value="active">Active</option>
                                            <option value="inactive">Inactive</option>
                                        </select>
                                        {formError.status && <div className="invalid-feedback">{formError.status}</div>}
                                    </div>
                                )}
                            </div>

                            {/* FOOTER */}
                            <div className="modal-footer border-top-0 pt-0">
                                <button
                                    type="button"
                                    className="btn btn-outline-secondary px-4"
                                    onClick={onHide}
                                    disabled={isSubmitting}
                                >
                                    Hủy
                                </button>

                                <button
                                    type="submit"
                                    className="btn btn-success px-4 d-flex align-items-center gap-2"
                                    disabled={isSubmitting || pickerLoading}
                                >
                                    {isSubmitting && (
                                        <span className="spinner-border spinner-border-sm"></span>
                                    )}
                                    {isSubmitting ? 'Đang lưu...' : 'Lưu danh mục'}
                                </button>
                            </div>
                        </form>

                    </div>
                </div>
            </div>
        </>
    );
};

export default CategoryFormModal;