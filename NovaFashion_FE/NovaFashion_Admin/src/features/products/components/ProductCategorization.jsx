import styles from '../../../pages/ProductDetails.module.css';

const ProductCategorization = ({
    currentCategoryName,
    categoryDropdowns,
    breadcrumbPath,
    handleCategorySelect,
    isChanging,
    setIsChanging,
    isLoading,
    isError }) => {


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

            {!isChanging && (
                <div className="d-flex align-items-center gap-2">
                    <i className="bi bi-tag text-muted"></i>
                    <span className="fw-semibold">{currentCategoryName ?? '—'}</span>
                </div>
            )}

            {/* Dropdown picker — giống CategorySection */}
            {isChanging && (
                <>
                    {isLoading && (
                        <div className="d-flex align-items-center gap-2 text-muted small mb-2">
                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true" />
                            <span>Đang tải danh mục...</span>
                        </div>
                    )}

                    {isError && (
                        <div className="text-danger small d-flex align-items-center gap-1 mb-2">
                            <i className="bi bi-exclamation-triangle-fill"></i>
                            <span>Không thể tải danh mục</span>
                        </div>
                    )}

                    {!isLoading && !isError && categoryDropdowns.map((dropdown, idx) => (
                        <div
                            key={dropdown.level}
                            className={idx < categoryDropdowns.length - 1 ? 'mb-3' : 'mb-0'}
                        >
                            <label className="form-label small fw-semibold">
                                {idx === 0 ? (
                                    <>Category <span className="text-danger">*</span></>
                                ) : (
                                    `Sub Category (cấp ${idx})`
                                )}
                            </label>
                            <select
                                value={dropdown.selectedId || ''}
                                onChange={(e) => handleCategorySelect(dropdown.level, e.target.value)}
                                className="form-select"
                            >
                                <option value="">
                                    {idx === 0 ? '-- Chọn category --' : '-- Chọn sub category --'}
                                </option>
                                {dropdown.options.map((opt) => (
                                    <option key={opt.id} value={opt.id}>
                                        {opt.category_name}
                                    </option>
                                ))}
                            </select>
                        </div>
                    ))}

                    {breadcrumbPath?.length > 0 && (
                        <div className={styles.breadcrumbPath} style={{ marginTop: '0.75rem' }}>
                            <i className="bi bi-diagram-3"></i>
                            {breadcrumbPath.map((node, idx) => (
                                <span key={node.id} className="d-flex align-items-center gap-1">
                                    {idx > 0 && (
                                        <i className="bi bi-chevron-right" style={{ fontSize: '0.65rem' }}></i>
                                    )}
                                    <span className={idx === breadcrumbPath.length - 1 ? 'fw-semibold' : ''}>
                                        {node.category_name}
                                    </span>
                                </span>
                            ))}
                        </div>
                    )}

                    <button
                        type="button"
                        className="btn btn-sm btn-light border mt-3"
                        onClick={() => setIsChanging(false)}
                    >
                        Cancel
                    </button>
                </>
            )}
        </div>
    );
};

export default ProductCategorization;