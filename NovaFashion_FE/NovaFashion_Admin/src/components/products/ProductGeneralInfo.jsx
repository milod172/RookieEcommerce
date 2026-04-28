import styles from '../../pages/ProductDetails.module.css';

const ProductGeneralInfo = ({ form, handleChange }) => {
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
                    className={`form-control ${styles.inlineInput}`}
                    rows={3}
                />
            </div>

            <div className="mb-0">
                <label htmlFor='details' className="form-label small fw-semibold">Details</label>
                <textarea
                    id="details"
                    name="details"
                    value={form.details || ''}
                    onChange={handleChange}
                    className={`form-control ${styles.inlineInput}`}
                    rows={5}
                />
            </div>
        </div>
    );
};

export default ProductGeneralInfo;