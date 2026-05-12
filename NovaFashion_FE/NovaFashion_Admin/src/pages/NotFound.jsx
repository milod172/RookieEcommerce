import { Link } from "react-router-dom";
import "./NotFound.css";

const NotFound = () => {
    return (
        <main className="notFound-content">
            <div className="notFound-card">
                <div className="notFound-illustration">
                    <div className="notFound-illustration__code">404</div>
                    <div className="notFound-illustration__icon">
                        <i className="bi bi-bag-x"></i>
                    </div>
                </div>

                <h2 className="notFound-title">Oops! Trang không tồn tại</h2>
                <p className="notFound-subtitle">
                    Trang bạn đang tìm kiếm có thể đã bị xóa, đổi tên hoặc tạm thời
                    không khả dụng. Hãy kiểm tra lại đường dẫn hoặc quay về trang chủ.
                </p>

                <div className="notFound-actions">
                    <Link to="/" className="notFound-btn notFound-btn--primary">
                        <i className="bi bi-house"></i> VỀ TRANG CHỦ
                    </Link>
                    <button
                        onClick={() => window.history.back()}
                        className="notFound-btn notFound-btn--secondary"
                    >
                        <i className="bi bi-arrow-left"></i> QUAY LẠI
                    </button>
                </div>
            </div>
        </main>
    )
}

export default NotFound;