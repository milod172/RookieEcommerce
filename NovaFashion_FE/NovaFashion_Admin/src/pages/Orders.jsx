import { Link } from "react-router-dom";
import styles from './Products.module.css';
import Pagination from "../components/Pagination";
import { useEffect, useState } from "react";
import { useOrders } from "../hooks/orders/useOrders";

const PAGE_SIZE = 5;

const Orders = () => {
    const [page, setPage] = useState(1);
    const [status, setStatus] = useState("All");
    const [sort, setSort] = useState("Newest");
    const [searchInput, setSearchInput] = useState("");
    const [search, setSearch] = useState("");

    const { orders, totalCount, isLoading } = useOrders({
        PageNumber: page,
        PageSize: PAGE_SIZE,
        SortBy: sort,
        OrderStatus: status,
        Search: search
    });

    //Search
    useEffect(() => {
        const handler = setTimeout(() => {
            setSearch(searchInput);
            setPage(1);
        }, 1000);

        return () => {
            clearTimeout(handler);
        };
    }, [searchInput]);

    const getStatusStyle = (status) => {
        switch (status) {
            case "Pending": return styles.pending;
            case "Paid": return styles.paid;
            case "Completed": return styles.completed;
            default: return "";
        }
    };

    if (isLoading) return <div className="p-3">Loading...</div>;

    return (
        <div className="container-fluid">
            <div className={`card border-0 shadow-sm ${styles.panel}`}>

                {/* Header */}
                <div className="d-flex align-items-center gap-5 mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">Orders Management</h5>
                        <small className="text-muted">
                            Quản lý danh sách đơn hàng
                        </small>
                    </div>

                    <div className={`ms-5 position-relative ${styles.searchBox}`}>
                        <i className="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>
                        <input
                            type="text"
                            className="form-control rounded-pill ps-5"
                            placeholder="Search..."
                            value={searchInput}
                            onChange={(e) => setSearchInput(e.target.value)}
                        />
                    </div>
                </div>

                {/* Filters */}
                <div className="d-flex flex-wrap gap-2 mb-3">
                    <select
                        className="form-select w-auto"
                        value={status}
                        onChange={(e) => {
                            setStatus(e.target.value);
                            setPage(1);
                        }}
                    >
                        <option value="All">Tất cả</option>
                        <option value="Pending">Pending</option>
                        <option value="Paid">Paid</option>
                        <option value="Completed">Completed</option>
                    </select>
                    <select
                        className="form-select w-auto"
                        value={sort}
                        onChange={(e) => {
                            setSort(e.target.value);
                            setPage(1);
                        }}
                    >
                        <option value="Newest">Sort by: Mới nhất</option>
                        <option value="Oldest">Sort by: Cũ nhất</option>
                        <option value="PriceAsc">Sort by: Giá tăng dần</option>
                        <option value="PriceDesc">Sort by: Giá giảm dần</option>
                    </select>
                </div>

                {/* Table */}
                <div className="table-responsive">
                    <table className="table align-middle mb-0">
                        <thead className="bg-light small text-uppercase text-muted">
                            <tr>
                                <th>ID</th>
                                <th>Order Date</th>
                                <th>Order By</th>
                                <th className="text-start">Total Amount</th>
                                <th className="text-center">Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {orders.map((p) => (
                                <tr key={p.id}>
                                    <td>{p.id.slice(0, 8)}</td>

                                    <td>
                                        <div className="d-flex align-items-center gap-3">
                                            <div>
                                                <div>
                                                    {p.order_date}
                                                </div>

                                            </div>
                                        </div>
                                    </td>

                                    <td>
                                        <span>{p.order_by}</span>
                                    </td>

                                    <td className="text-start">
                                        {p.total_amount?.toLocaleString("vi-VN", {
                                            style: "currency",
                                            currency: "VND"
                                        })}</td>

                                    <td className="text-center">
                                        <span className={`${styles.status} ${getStatusStyle(p.order_status)}`}>
                                            {p.order_status}
                                        </span>
                                    </td>

                                    <td className="text-end">
                                        <Link
                                            to={`${p.id}`}

                                            className={`btn btn-light btn-sm ${styles.actionBtn}`}
                                        >
                                            <i className="bi bi-eye"></i>
                                        </Link>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                <Pagination
                    page={page}
                    totalCount={totalCount}
                    pageSize={PAGE_SIZE}
                    onPageChange={setPage}
                    itemLabel="đơn hàng"
                />
            </div>
        </div>
    );
}

export default Orders;