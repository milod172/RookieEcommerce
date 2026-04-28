import styles from '../../../pages/CreateProduct.module.css';

const CategorySection = ({ isLoading, isError, categoryDropdowns, breadcrumbPath, handleCategorySelect }) => {
    if (isLoading) return (
        <div className="d-flex align-items-center gap-2 text-muted small">
            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true" />
            <span>Đang tải danh mục...</span>
        </div>
    );

    if (isError) return (
        <div className="text-danger small d-flex align-items-center gap-1">
            <i className="bi bi-exclamation-triangle-fill"></i>
            <span>Không thể tải danh mục</span>
        </div>
    );

    return (
        <>
            {categoryDropdowns.map((dropdown, idx) => (
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
                        className="form-select"
                        value={dropdown.selectedId}
                        onChange={(e) => handleCategorySelect(dropdown.level, e.target.value)}
                        required={idx === 0}
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

            {breadcrumbPath && (
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
        </>
    );
};

export default CategorySection;