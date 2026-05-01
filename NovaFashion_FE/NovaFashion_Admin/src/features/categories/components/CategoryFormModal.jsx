/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { useState, useEffect, useMemo } from 'react';
import { useCategoriesPicker, useCreateCategory } from '../../../hooks/categories/useCategory';


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
    show,
    onHide,
    onSuccess,
    initialValues = EMPTY,
    title = 'Thêm danh mục',
}) => {

    const [name, setName] = useState(initialValues.name ?? '');
    const [description, setDescription] = useState(initialValues.description ?? '');
    const [parentId, setParentId] = useState(initialValues.parentId ?? '');
    const [formError, setFormError] = useState('');

    // Reset whenever the modal opens
    useEffect(() => {
        if (show) {
            setName(initialValues.name ?? '');
            setDescription(initialValues.description ?? '');
            setParentId(initialValues.parentId ?? '');
            setFormError('');
        }
    }, [show]);


    const { pickerItems, isLoading: pickerLoading, isError: pickerError } = useCategoriesPicker();
    const { createCategory, isCreating, createError } = useCreateCategory();

    const flatItems = useMemo(() => flattenPickerItems(pickerItems), [pickerItems]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFormError('');

        if (!name.trim()) {
            setFormError('Tên danh mục không được để trống.');
            return;
        }

        try {
            await createCategory({
                category_name: name.trim(),
                description: description.trim(),
                parent_category_id: parentId || null,
            });

            onSuccess?.();
            onHide?.();
        } catch (err) {
            setFormError(
                err?.response?.data?.message ||
                'Có lỗi xảy ra khi tạo danh mục. Vui lòng thử lại.'
            );
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
                                disabled={isCreating}
                            />
                        </div>

                        {/* BODY */}
                        <form onSubmit={handleSubmit}>
                            <div className="modal-body pt-2">

                                {/* ERROR */}
                                {(formError || createError) && (
                                    <div className="alert alert-danger py-2 px-3 small">
                                        {formError || 'Có lỗi xảy ra khi tạo danh mục.'}
                                    </div>
                                )}

                                {/* NAME */}
                                <div className="mb-3">
                                    <label className="form-label small fw-medium">
                                        Tên danh mục <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        type="text"
                                        className={`form-control ${!name.trim() && formError ? 'is-invalid' : ''}`}
                                        value={name}
                                        onChange={(e) => setName(e.target.value)}
                                        disabled={isCreating}
                                        autoFocus
                                    />
                                    <div className="invalid-feedback">
                                        Vui lòng nhập tên danh mục.
                                    </div>
                                </div>

                                {/* DESCRIPTION */}
                                <div className="mb-3">
                                    <label className="form-label small fw-medium">Mô tả</label>
                                    <textarea
                                        className="form-control"
                                        rows={3}
                                        value={description}
                                        onChange={(e) => setDescription(e.target.value)}
                                        disabled={isCreating}
                                        style={{ resize: 'none' }}
                                    />
                                </div>

                                {/* PARENT */}
                                <div>
                                    <label className="form-label small fw-medium">Danh mục cha</label>

                                    {pickerError ? (
                                        <div className="alert alert-warning py-2 px-3 small">
                                            Không thể tải danh sách danh mục.
                                        </div>
                                    ) : (
                                        <div className="position-relative">
                                            <select
                                                className="form-select"
                                                value={parentId}
                                                onChange={(e) => setParentId(e.target.value)}
                                                disabled={isCreating || pickerLoading}
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
                            </div>

                            {/* FOOTER */}
                            <div className="modal-footer border-top-0 pt-0">
                                <button
                                    type="button"
                                    className="btn btn-outline-secondary px-4"
                                    onClick={onHide}
                                    disabled={isCreating}
                                >
                                    Hủy
                                </button>

                                <button
                                    type="submit"
                                    className="btn btn-success px-4 d-flex align-items-center gap-2"
                                    disabled={isCreating || pickerLoading}
                                >
                                    {isCreating && (
                                        <span className="spinner-border spinner-border-sm"></span>
                                    )}
                                    {isCreating ? 'Đang lưu...' : 'Lưu danh mục'}
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