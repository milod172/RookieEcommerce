import { Link } from 'react-router-dom';
import styles from '../../pages/ProductDetails.module.css';

const ProductHeader = ({ form, isDirty, handleDiscard }) => {
    return (
        <>
            {/* Top header */}
            <div className="d-flex justify-content-between align-items-center mb-3">
                <div>
                    <div className="d-flex align-items-center gap-2 text-muted small mb-1">
                        <i className="bi bi-box-seam"></i>
                        <Link to="/products" className={styles.backDecorationNone}>
                            <span>Products</span>
                        </Link>
                        <i className="bi bi-chevron-right" style={{ fontSize: '0.7rem' }}></i>
                        <span className="text-dark">{form.name || 'Product details'}</span>
                    </div>
                    <div className="d-flex align-items-center gap-2">
                        <h5 className="fw-bold mb-0">{form.name || 'Product details'}</h5>
                        <span
                            className={`${styles.statusPill} ${form.status === 'active' ? styles.active : styles.inactive}`}
                        >
                            {form.status === 'active' ? 'Active' : 'Inactive'}
                        </span>
                    </div>
                    <small className="text-muted">
                        ID: <span className="fw-semibold">{form.id}</span> · SKU:{' '}
                        <span className="fw-semibold">{form.sku}</span>
                    </small>
                </div>

                <div className="d-flex gap-2 align-items-center">
                    {isDirty && (
                        <span className={styles.dirtyDot}>
                            <i className="bi bi-circle-fill"></i> Có thay đổi chưa lưu
                        </span>
                    )}
                    <button
                        type="button"
                        className="btn btn-light border"
                        onClick={handleDiscard}
                        disabled={!isDirty}
                    >
                        Discard
                    </button>
                    <button
                        type="submit"
                        className={`btn ${styles.btnAccent}`}
                        disabled={!isDirty}
                    >
                        <i className="bi bi-check2"></i> Save Changes
                    </button>
                </div>
            </div>
        </>
    );
};

export default ProductHeader;