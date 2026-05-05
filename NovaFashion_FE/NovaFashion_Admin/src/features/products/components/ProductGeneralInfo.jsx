import styles from '../../../pages/ProductDetails.module.css';

const ProductGeneralInfo = ({ form, handleChange, fieldErrors }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <h6 className={styles.sectionTitle}>General Information</h6>

            <div className="mb-3">
                <label htmlFor='name' className="form-label small fw-semibold">Product Name</label>
                <input
                    type="text"
                    id="name"
                    name="name"
                    value={form.name || ''}
                    onChange={handleChange}
                    className={`form-control ${styles.inlineInput}`}
                />
            </div>

            <div className="mb-3">
                <label htmlFor='description' className="form-label small fw-semibold">Description</label>
                <textarea
                    id="description"
                    name="description"
                    value={form.description || ''}
                    onChange={handleChange}
                    className={`form-control ${styles.inlineInput} ${fieldErrors.description ? 'is-invalid' : ''}`}
                    rows={3}
                />
                {fieldErrors.description && (
                    <div className="invalid-feedback">
                        {fieldErrors.description}
                    </div>
                )}
            </div>

            <div className="mb-0">
                <label htmlFor='details' className="form-label small fw-semibold">Details</label>
                <textarea
                    id="details"
                    name="details"
                    value={form.details || ''}
                    onChange={handleChange}
                    className={`form-control ${styles.inlineInput} ${fieldErrors.details ? 'is-invalid' : ''}`}
                    rows={5}
                />
                {fieldErrors.details && (
                    <div className="invalid-feedback">
                        {fieldErrors.details}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ProductGeneralInfo;