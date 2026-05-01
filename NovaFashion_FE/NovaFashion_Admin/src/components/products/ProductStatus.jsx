import styles from '../../pages/ProductDetails.module.css';

const ProductStatus = ({ form, handleChange }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <h6 className={styles.sectionTitle}>Status</h6>
            <select
                name="isDeleted"
                value={form.isDeleted ? 'inactive' : 'active'}
                onChange={(e) =>
                    handleChange({
                        target: {
                            name: 'isDeleted',
                            value: e.target.value === 'inactive',
                        },
                    })
                }
                className={`form-select ${styles.inlineInput}`}
            >
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
            </select>
            <small className="text-muted d-block mt-2">
                Sản phẩm "Inactive" sẽ không hiển thị ngoài cửa hàng.
            </small>
        </div>
    );
};

export default ProductStatus;