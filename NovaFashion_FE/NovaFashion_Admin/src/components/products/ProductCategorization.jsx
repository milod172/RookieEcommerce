import { CATEGORIES } from '../../constants/productConstants';
import styles from '../../pages/ProductDetails.module.css';

const ProductCategorization = ({ form, subCategories, handleChange, handleCategoryChange }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <h6 className={styles.sectionTitle}>Categorization</h6>

            <div className="mb-3">
                <label className="form-label small fw-semibold">Category</label>
                <select
                    name="categoryId"
                    value={form.categoryId || ''}
                    onChange={handleCategoryChange}
                    className={`form-select ${styles.inlineInput}`}
                >
                    <option value="">-- Chọn category --</option>
                    {CATEGORIES.map((c) => (
                        <option key={c.id} value={c.id}>
                            {c.name}
                        </option>
                    ))}
                </select>
            </div>

            <div className="mb-0">
                <label className="form-label small fw-semibold">Sub Category</label>
                <select
                    name="subCategoryId"
                    value={form.subCategoryId || ''}
                    onChange={handleChange}
                    className={`form-select ${styles.inlineInput}`}
                    disabled={!form.categoryId || subCategories.length === 0}
                >
                    <option value="">
                        {!form.categoryId
                            ? '-- Chọn category trước --'
                            : subCategories.length === 0
                                ? '-- Không có sub category --'
                                : '-- Chọn sub category --'}
                    </option>
                    {subCategories.map((s) => (
                        <option key={s.id} value={s.id}>
                            {s.name}
                        </option>
                    ))}
                </select>

                {form.categoryId && (
                    <div className={styles.breadcrumbPath}>
                        <i className="bi bi-diagram-3"></i>
                        <span>{CATEGORIES.find((c) => c.id === form.categoryId)?.name}</span>
                        {form.subCategoryId && (
                            <>
                                <i className="bi bi-chevron-right" style={{ fontSize: '0.65rem' }}></i>
                                <span className="fw-semibold">
                                    {subCategories.find((s) => s.id === form.subCategoryId)?.name}
                                </span>
                            </>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ProductCategorization;