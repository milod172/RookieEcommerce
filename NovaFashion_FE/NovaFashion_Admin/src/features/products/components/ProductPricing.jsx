import styles from '../../../pages/ProductDetails.module.css';

const ProductPricing = ({ form, handleChange, fieldErrors }) => {
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
                        className={`form-control ${styles.inlineInput} ${fieldErrors.basePrice ? 'is-invalid' : ''}`}
                        min="0"
                    />
                    {fieldErrors.basePrice && (
                        <div className="invalid-feedback">
                            {fieldErrors.basePrice}
                        </div>
                    )}
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
                    className={`form-control ${styles.inlineInput} ${fieldErrors.totalQuantity ? 'is-invalid' : ''}`}
                    min="0"
                />
                {fieldErrors.totalQuantity && (
                    <div className="invalid-feedback mb-2">
                        {fieldErrors.totalQuantity}
                    </div>
                )}
                <small className="text-muted">Tổng tồn kho = tổng stock của các variants.</small>
            </div>
        </div>
    );
};

export default ProductPricing;