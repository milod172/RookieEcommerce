import styles from '../../pages/ProductDetails.module.css';

const ProductCategorization = ({ currentCategoryName, categoryDropdowns, breadcrumbPath, handleCategorySelect, isChanging, setIsChanging }) => {


    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h6 className={`${styles.sectionTitle} mb-0`}>Categorization</h6>
                {!isChanging && (
                    <button
                        type="button"
                        className="btn btn-sm btn-light border"
                        onClick={() => setIsChanging(true)}
                    >
                        <i className="bi bi-pencil me-1"></i>Change
                    </button>
                )}
            </div>

            {isChanging ? (
                <>
                    {categoryDropdowns.map((dropdown) => (
                        <div key={dropdown.level} className="mb-2">
                            <label className="form-label small fw-semibold">
                                Level {dropdown.level + 1}
                            </label>

                            {dropdown.isLoading ? (
                                <div className="d-flex align-items-center gap-2 text-muted small">
                                    <span className="spinner-border spinner-border-sm" />
                                    Đang tải...
                                </div>
                            ) : (
                                <select
                                    value={dropdown.selectedId || ''}
                                    onChange={(e) => handleCategorySelect(dropdown.level, e.target.value)}
                                    className={`form-select ${styles.inlineInput}`}
                                >
                                    <option value="">-- Chọn --</option>
                                    {dropdown.options.map((opt) => (
                                        <option key={opt.id} value={opt.id}>
                                            {opt.category_name}
                                        </option>
                                    ))}
                                </select>
                            )}
                        </div>
                    ))}

                    {breadcrumbPath?.length > 0 && (
                        <div className={styles.breadcrumbPath}>
                            <i className="bi bi-diagram-3"></i>
                            {breadcrumbPath.map((node, idx) => (
                                <span key={node.id}>
                                    {idx > 0 && <i className="bi bi-chevron-right"></i>}
                                    {node.category_name}
                                </span>
                            ))}
                        </div>
                    )}

                    <button
                        type="button"
                        className="btn btn-sm btn-light border mt-2"
                        onClick={() => setIsChanging(false)}
                    >
                        Cancel
                    </button>
                </>
            ) : (

                <div className="d-flex align-items-center gap-2">
                    <i className="bi bi-tag text-muted"></i>
                    <span className="fw-semibold">{currentCategoryName ?? '—'}</span>
                </div>
            )}
        </div>
    );
};

export default ProductCategorization;