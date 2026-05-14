import "./Dashboard.css";
const Dashboard = () => {
    return (
        <div class="dsb-page">
            <div className="dsb-welcome">
                <span className="dsb-welcome__bubble dsb-welcome__bubble--1"></span>
                <span className="dsb-welcome__bubble dsb-welcome__bubble--2"></span>
                <span className="dsb-welcome__bubble dsb-welcome__bubble--3"></span>

                <div className="dsb-welcome__content">
                    <span className="dsb-welcome__badge">
                        <i className="fa-solid fa-hand-sparkles"></i> Xin chào lần nữa
                    </span>
                    <h2 className="dsb-welcome__title">
                        Chào mừng bạn <span className="dsb-welcome__title-accent">quay trở lại!</span>
                    </h2>
                    <p className="dsb-welcome__subtitle">
                        Đây là không gian làm việc của bạn. Hiện chưa có dữ liệu để hiển thị —
                        hãy bắt đầu bằng cách thêm sản phẩm hoặc kết nối nguồn dữ liệu để xem
                        các chỉ số hấp dẫn xuất hiện ở đây.
                    </p>
                </div>
            </div>
        </div>

    );
};

export default Dashboard;