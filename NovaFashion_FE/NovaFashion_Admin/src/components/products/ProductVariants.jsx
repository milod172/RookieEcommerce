import { formatVnd } from '../../utils/formatters';
import styles from '../../pages/ProductDetails.module.css';
import React from 'react';

const StatusBadge = ({ status }) => {
    const isActive = status === 'Active';
    return (
        <span className={`${styles.variantBadge} ${isActive ? styles.active : styles.inactive}`}>
            {isActive ? 'Active' : 'InActive'}
        </span>
    );
};

const ProductVariants = ({
    variants,
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
}) => {
    const isAnyEditing = editingVariantId !== null || addingRow;

    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <div className="d-flex justify-content-between align-items-center mb-2">
                <h6 className={`${styles.sectionTitle} mb-0`}>Variants ({variants.length})</h6>

                <div className="d-flex align-items-center gap-2">
                    <small className="text-muted">
                        Click <i className="bi bi-pencil-square"></i> để chỉnh sửa trực tiếp
                    </small>
                    <button
                        type="button"
                        className={`btn btn-light btn-sm ${styles.actionBtn}`}
                        onClick={startAddVariant}
                        disabled={isAnyEditing}
                        title="Thêm variant mới"
                    >
                        <i className="bi bi-plus-lg"></i>
                    </button>
                </div>
            </div>

            {isLoadingVariants ? (
                <div className="text-center py-4 text-muted">
                    <div className="spinner-border spinner-border-sm me-2" role="status" />
                    Đang tải variants...
                </div>
            ) : (
                <div className="table-responsive">
                    <table className={`table align-middle mb-0 ${styles.variantTable}`}>
                        <thead>
                            <tr>
                                <th>SKU</th>
                                <th>Size</th>
                                <th className="text-end">Stock Quantity</th>
                                <th className="text-end">Unit Price</th>
                                <th className="text-end">Status</th>
                                <th className="text-center" style={{ width: 120 }}>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {variants.map((v) => {
                                const isEditing = editingVariantId === v.id;
                                return (
                                    <React.Fragment key={v.id}>
                                        <tr className={isEditing ? styles.editingRow : ''}>
                                            <td>
                                                <span className="fw-semibold">{v.skuVariant}</span>
                                            </td>
                                            <td>
                                                <small className="text-muted">{v.attrs}</small>
                                            </td>

                                            {/* Stock Quantity */}
                                            <td className="text-end">
                                                {isEditing ? (
                                                    <div className={styles.errorWrapper}>
                                                        <input
                                                            type="number"
                                                            name="stockQuantity"
                                                            value={variantDraft?.stockQuantity || 0}
                                                            onChange={handleVariantDraftChange}
                                                            className={`form-control form-control-sm text-end ${editFieldErrors?.stockQuantity ? 'is-invalid' : ''}`}
                                                            min="0"
                                                        />

                                                        {editFieldErrors?.stockQuantity && (
                                                            <div className={styles.errorSpace}>
                                                                <div className="invalid-feedback d-block text-start">
                                                                    {editFieldErrors.stockQuantity}
                                                                </div>
                                                            </div>
                                                        )}

                                                    </div>
                                                ) : (
                                                    v.stockQuantity
                                                )}
                                            </td>

                                            {/* Unit Price */}
                                            <td className="text-end">
                                                {isEditing ? (
                                                    <div className={styles.errorWrapper}>
                                                        <input
                                                            type="number"
                                                            name="unitPrice"
                                                            value={variantDraft?.unitPrice || 0}
                                                            onChange={handleVariantDraftChange}
                                                            className={`form-control form-control-sm text-end ${editFieldErrors?.unitPrice ? 'is-invalid' : ''}`}
                                                            min="0"
                                                        />

                                                        {editFieldErrors?.unitPrice && (
                                                            <div className={styles.errorSpace}>
                                                                <div className="invalid-feedback d-block text-start">
                                                                    {editFieldErrors.unitPrice}
                                                                </div>
                                                            </div>
                                                        )}

                                                    </div>
                                                ) : (
                                                    formatVnd(v.unitPrice)
                                                )}
                                            </td>

                                            {/* Status */}
                                            <td className="text-end">
                                                {isEditing ? (
                                                    <select
                                                        name="status"
                                                        value={variantDraft?.status}
                                                        onChange={handleVariantDraftChange}
                                                        className="form-select form-select-sm"
                                                        style={{ width: 110, margin: '0 auto' }}
                                                    >
                                                        <option value="Active">Active</option>
                                                        <option value="InActive">InActive</option>
                                                    </select>

                                                ) : (
                                                    <StatusBadge status={v.status} />
                                                )}
                                            </td>

                                            {/* Action */}
                                            <td className="text-center">
                                                {isEditing ? (
                                                    <div className="d-flex justify-content-center gap-1">
                                                        <button
                                                            type="button"
                                                            className={`btn btn-sm ${styles.btnAccent}`}
                                                            onClick={saveEditVariant}
                                                        >
                                                            <i className="bi bi-check-lg"></i>
                                                        </button>
                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-light border"
                                                            onClick={cancelEditVariant}
                                                        >
                                                            <i className="bi bi-x-lg"></i>
                                                        </button>
                                                    </div>
                                                ) : (
                                                    <button
                                                        type="button"
                                                        className={`btn btn-sm btn-light border ${styles.editBtn}`}
                                                        onClick={() => startEditVariant(v)}
                                                        disabled={isAnyEditing}
                                                    >
                                                        <i className="bi bi-pencil-square"></i> Edit
                                                    </button>
                                                )}
                                            </td>
                                        </tr>

                                        {/* General error row — ngay dưới row đang edit */}
                                        {isEditing && editError && (
                                            <tr>
                                                <td colSpan={6} className="pt-0 pb-1 border-0">
                                                    <div className="text-danger small px-1">
                                                        <i className="bi bi-exclamation-circle me-1"></i>
                                                        {editError}
                                                    </div>
                                                </td>
                                            </tr>
                                        )}
                                    </React.Fragment>
                                );
                            })}

                            {/* ── Inline Add Row ── */}
                            {addingRow && (
                                <>
                                    <tr className={styles.editingRow}>

                                        <td>
                                            <span className="text-muted fst-italic" style={{ fontSize: '0.8rem' }}>
                                                Auto-generated
                                            </span>
                                        </td>


                                        <td>
                                            <div className={styles.errorWrapper}>
                                                <select
                                                    name="size"
                                                    value={addDraft.size}
                                                    onChange={handleAddDraftChange}
                                                    className={`form-select form-select-sm ${addFieldErrors.size ? 'is-invalid' : ''}`}
                                                    style={{ width: 90 }}
                                                >
                                                    {SIZE_OPTIONS.map((s) => (
                                                        <option key={s} value={s}>{s}</option>
                                                    ))}
                                                </select>

                                                {addFieldErrors.size && (
                                                    <div className={styles.errorSpace}>
                                                        <div className="invalid-feedback d-block">
                                                            {addFieldErrors.size}
                                                        </div>
                                                    </div>
                                                )}

                                            </div>
                                        </td>


                                        <td className="text-end">
                                            <div className={styles.errorWrapper}>
                                                <input
                                                    type="number"
                                                    name="stockQuantity"
                                                    value={addDraft.stockQuantity}
                                                    onChange={handleAddDraftChange}
                                                    className={`form-control form-control-sm text-end ${addFieldErrors.stockQuantity ? 'is-invalid' : ''}`}
                                                    min="0"
                                                />

                                                {addFieldErrors.stockQuantity && (
                                                    <div className={styles.errorSpace}>
                                                        <div className="invalid-feedback d-block">
                                                            {addFieldErrors.stockQuantity}
                                                        </div>
                                                    </div>
                                                )}

                                            </div>

                                        </td>


                                        <td className="text-end">

                                            <div className={styles.errorWrapper}>
                                                <input
                                                    type="number"
                                                    name="unitPrice"
                                                    value={addDraft.unitPrice}
                                                    onChange={handleAddDraftChange}
                                                    className={`form-control form-control-sm text-end ${addFieldErrors.unitPrice ? 'is-invalid' : ''}`}
                                                    min="0"
                                                />

                                                {addFieldErrors.unitPrice && (
                                                    <div className={styles.errorSpace}>
                                                        <div className="invalid-feedback d-block">
                                                            {addFieldErrors.unitPrice}
                                                        </div>
                                                    </div>
                                                )}

                                            </div>

                                        </td>
                                        <td></td>

                                        <td className="text-center">
                                            <div className="d-flex justify-content-center gap-1">
                                                <button
                                                    type="button"
                                                    className={`btn btn-sm ${styles.btnAccent}`}
                                                    onClick={saveAddVariant}
                                                >
                                                    <i className="bi bi-check-lg"></i>
                                                </button>
                                                <button
                                                    type="button"
                                                    className="btn btn-sm btn-light border"
                                                    onClick={cancelAddVariant}
                                                >
                                                    <i className="bi bi-x-lg"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>

                                    {addError && (
                                        <tr>
                                            <td colSpan={5} className="pt-0 pb-1 border-0">
                                                <div className="text-danger small px-1">
                                                    <i className="bi bi-exclamation-circle me-1"></i>
                                                    {addError}
                                                </div>
                                            </td>
                                        </tr>
                                    )}
                                </>
                            )}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default ProductVariants;