import styles from './StatCard.module.css';

const StatCard = ({ title, value, icon }) => (
    <div className="card border-0 shadow-sm rounded-4 h-100">
        <div className="card-body d-flex align-items-center p-3">

            <div className={`${styles.iconContainer} d-flex align-items-center justify-content-center me-3`}>
                <span className={styles.icon}>{icon}</span>
            </div>

            <div className="d-flex flex-column">
                <span className="text-muted small fw-medium mb-1">{title}</span>
                <h4 className="fw-bold mb-0 text-dark">{value}</h4>
            </div>
        </div>
    </div>
);

export default StatCard;