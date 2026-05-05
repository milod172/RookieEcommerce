import styles from '../../../pages/ProductDetails.module.css';

const ProductStats = ({ form }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            <h6 className={styles.sectionTitle}>Performance</h6>
            <div className="row g-2">
                <div className="col-6">
                    <div className={styles.statBox}>
                        <small className="text-muted">Total Quantity</small>
                        <div className="fw-bold fs-5">{form.totalQuantity}</div>
                    </div>
                </div>
                <div className="col-6">
                    <div className={styles.statBox}>
                        <small className="text-muted">Total Sell</small>
                        <div className="fw-bold fs-5">{form.totalSell}</div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProductStats;