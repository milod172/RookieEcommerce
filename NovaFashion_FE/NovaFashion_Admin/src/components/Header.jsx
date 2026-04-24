import styles from './Header.module.css';

const Header = ({ title }) => {
    return (
        <header className="d-flex align-items-center justify-content-between mb-4">
            <h4 className="fw-bold mb-0">{title}</h4>

            <div className="d-flex align-items-center gap-2">
                <div className={`position-relative ${styles.searchBox}`}>
                    <i className="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>
                    <input
                        type="text"
                        className="form-control rounded-pill ps-5"
                        placeholder="Search..."
                    />
                </div>

                <button className="btn btn-light border rounded-circle">
                    <i className="bi bi-globe"></i>
                </button>

                <button className="btn btn-light border rounded-circle">
                    <i className="bi bi-grid-3x3-gap"></i>
                </button>

                <img
                    src="https://i.pravatar.cc/40?img=12"
                    className="rounded-circle"
                    style={{ width: "2.375rem", height: "2.375rem", objectFit: "cover" }}
                    alt="user"
                />
            </div>
        </header>
    );
};

export default Header;