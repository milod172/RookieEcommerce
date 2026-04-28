import { formatVnd } from '../../utils/formatters';
import styles from '../../pages/ProductDetails.module.css';

const ProductVariants = ({ variants, editingVariantId, variantDraft, startEditVariant, cancelEditVariant, saveEditVariant, handleVariantDraftChange }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <div className="d-flex justify-content-between align-items-center mb-2">
                <h6 className={`${styles.sectionTitle} mb-0`}>Variants ({variants.length})</h6>
                <small className="text-muted">
                    Click <i className="bi bi-pencil-square"></i> để chỉnh sửa trực tiếp
                </small>
            </div>

            <div className="table-responsive">
                <table className={`table align-middle mb-0 ${styles.variantTable}`}>
                    <thead>
                        <tr>
                            <th>SKU Variant</th>
                            <th>Thuộc tính</th>
                            <th className="text-end">Stock Quantity</th>
                            <th className="text-end">Unit Price</th>
                            <th className="text-center" style={{ width: 120 }}>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {variants.map((v) => {
                            const isEditing = editingVariantId === v.id;
                            return (
                                <tr key={v.id} className={isEditing ? styles.editingRow : ''}>
                                    <td>
                                        {isEditing ? (
                                            <input
                                                type="text"
                                                name="skuVariant"
                                                value={variantDraft?.skuVariant || ''}
                                                onChange={handleVariantDraftChange}
                                                className="form-control form-control-sm"
                                            />
                                        ) : (
                                            <span className="fw-semibold">{v.skuVariant}</span>
                                        )}
                                    </td>
                                    <td>
                                        <small className="text-muted">{v.attrs}</small>
                                    </td>
                                    <td className="text-end">
                                        {isEditing ? (
                                            <input
                                                type="number"
                                                name="stockQuantity"
                                                value={variantDraft?.stockQuantity || 0}
                                                onChange={handleVariantDraftChange}
                                                className="form-control form-control-sm text-end"
                                                min="0"
                                            />
                                        ) : (
                                            v.stockQuantity
                                        )}
                                    </td>
                                    <td className="text-end">
                                        {isEditing ? (
                                            <input
                                                type="number"
                                                name="unitPrice"
                                                value={variantDraft?.unitPrice || 0}
                                                onChange={handleVariantDraftChange}
                                                className="form-control form-control-sm text-end"
                                                min="0"
                                            />
                                        ) : (
                                            formatVnd(v.unitPrice)
                                        )}
                                    </td>
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
                                                disabled={editingVariantId !== null && editingVariantId !== v.id}
                                            >
                                                <i className="bi bi-pencil-square"></i> Edit
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ProductVariants;