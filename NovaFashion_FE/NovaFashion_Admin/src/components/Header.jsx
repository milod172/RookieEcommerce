import { useNavigate } from 'react-router-dom';
import styles from './Header.module.css';
import { authApi } from '../features/authentications/authApi';

const Header = ({ title }) => {
    const navigate = useNavigate();

    const handleLogout = () => {
        const confirmed = globalThis.confirm("Bạn có muốn đăng xuất không?");
        if (confirmed) {
            authApi.logout();
            navigate("/login");
        }
    };

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

                <img
                    src="https://i.pravatar.cc/40?img=12"
                    className="rounded-circle"
                    style={{ width: "2.375rem", height: "2.375rem", objectFit: "cover" }}
                    alt="user"
                />

                <button
                    className="btn btn-link text-danger text-decoration-none p-0 fw-semibold"
                    onClick={handleLogout}
                >
                    <i className="bi bi-box-arrow-right me-1"></i>
                    Đăng xuất
                </button>
            </div>
        </header>
    );
};

export default Header;