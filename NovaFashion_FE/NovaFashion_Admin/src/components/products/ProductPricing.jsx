import styles from '../../pages/ProductDetails.module.css';

const ProductPricing = ({ form, handleChange }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <h6 className={styles.sectionTitle}>Pricing & Inventory</h6>

            <div className="mb-3">
                <label htmlFor='basePrice' className="form-label small fw-semibold">Base Price</label>
                <div className="input-group">
                    <span className="input-group-text">₫</span>
                    <input
                        id="basePrice"
                        type="number"
                        name="basePrice"
                        value={form.basePrice || 0}
                        onChange={handleChange}
                        className={`form-control ${styles.inlineInput}`}
                        min="0"
                    />
                </div>
            </div>

            <div className="mb-0">
                <label htmlFor='totalQuantity' className="form-label small fw-semibold">Total Quantity</label>
                <input
                    id="totalQuantity"
                    type="number"
                    name="totalQuantity"
                    value={form.totalQuantity || 0}
                    onChange={handleChange}
                    className={`form-control ${styles.inlineInput}`}
                    min="0"
                />
                <small className="text-muted">Tổng tồn kho = tổng stock của các variants.</small>
            </div>
        </div>
    );
};

export default ProductPricing;