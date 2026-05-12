import { Link, useParams } from "react-router-dom";
import "./OrderDetails.css";
import { useOrderDetails } from "../hooks/orders/useOrders";

const OrderDetails = () => {
    const { id } = useParams();
    const { order, isLoading, isError } = useOrderDetails(id);

    if (isLoading) return <div>Loading...</div>;
    if (isError || !order) return <div>Không tìm thấy đơn hàng</div>;
    return (
        <main className="orderDetails-content">
            <div className="orderDetails-card">
                <div className="orderDetails-header">
                    <h2 className="orderDetails-header__title">Chi tiết đơn hàng</h2>
                    <div className="orderDetails-header__code">
                        MÃ ĐƠN:{" "}
                        <span className="orderDetails-header__codeValue">
                            #{order.id}
                        </span>
                    </div>
                </div>

                <div className="row g-4">
                    {/* Left column */}
                    <div className="col-lg-7">
                        <div className="orderDetails-section__heading">
                            THÔNG TIN KHÁCH HÀNG
                        </div>
                        <div className="orderDetails-infoBox">
                            <div className="row">
                                <div className="col-md-6">
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">HỌ VÀ TÊN</div>
                                        <div className="orderDetails-field__value">{order.full_name}</div>
                                    </div>
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">
                                            ĐỊA CHỈ GIAO HÀNG
                                        </div>
                                        <div className="orderDetails-field__value">
                                            {order.address}
                                        </div>
                                    </div>
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">NGÀY TẠO</div>
                                        <div className="orderDetails-field__value">
                                            {order.create_time}
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-6">
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">
                                            SỐ ĐIỆN THOẠI
                                        </div>
                                        <div className="orderDetails-field__value">{order.phone_number}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="orderDetails-section__heading">
                            THÔNG TIN THANH TOÁN
                        </div>
                        <div className="orderDetails-infoBox">
                            <div className="row">
                                <div className="col-md-6">
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">PHƯƠNG THỨC</div>
                                        <div className="orderDetails-field__value">{order.payment_method}</div>
                                    </div>
                                </div>
                                <div className="col-md-6">
                                    <div className="orderDetails-field">
                                        <div className="orderDetails-field__label">TRẠNG THÁI</div>
                                        <span className="orderDetails-status">{order.order_status}</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <Link to="/orders" className="orderDetails-backBtn text-decoration-none">
                            QUAY LẠI DANH SÁCH
                        </Link>

                    </div>

                    {/* Right column */}
                    <div className="col-lg-5">
                        <div className="orderDetails-summary">
                            <div className="orderDetails-summary__title">
                                SẢN PHẨM ĐÃ ĐẶT ({order?.order_items?.length || 0})
                            </div>
                            {order?.order_items?.map((item) => (
                                <div key={item.product_variant_id} className="orderDetails-product">
                                    <Link to={`/products/${item.product_id}`}>
                                        <div
                                            className="orderDetails-product__image"
                                            style={{ backgroundImage: `url(${item.image_url})` }}
                                        />
                                    </Link>

                                    <div>
                                        <div className="orderDetails-product__name">
                                            {item.product_name}
                                        </div>
                                        <div className="orderDetails-product__meta">
                                            Size: {item.size} · Sl: {item.quantity}
                                        </div>
                                        <div className="orderDetails-product__price">
                                            {item.total_price.toLocaleString("vi-VN")} ₫
                                        </div>
                                    </div>
                                </div>
                            ))}


                            <div className="orderDetails-summary__total">
                                <span className="orderDetails-summary__totalLabel">
                                    TỔNG CỘNG
                                </span>
                                <span className="orderDetails-summary__totalValue">
                                    {order?.total_amount?.toLocaleString("vi-VN")} ₫
                                </span>
                            </div>


                        </div>
                    </div>
                </div>
            </div>
        </main>
    );
}

export default OrderDetails;