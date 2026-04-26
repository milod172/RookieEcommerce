import { Link } from 'react-router-dom';
import styles from './Products.module.css';
import { useMemo, useState } from 'react';
import { useProducts } from '../hooks/products/useProducts.js';

const PAGE_SIZE = 5;

const Products = () => {
    const [page, setPage] = useState(1);

    // API
    const { products, totalCount, isLoading } = useProducts({
        PageNumber: page,
        PageSize: PAGE_SIZE,
        SortBy: "Id desc",
        IncludeDeleted: false,
    });

    const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

    const goTo = (p) => {
        const next = Math.min(Math.max(1, p), totalPages);
        setPage(next);
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    // Pagination UI
    const pageNumbers = useMemo(() => {
        const pages = [];
        const windowSize = 1;

        for (let i = 1; i <= totalPages; i++) {
            if (
                i === 1 ||
                i === totalPages ||
                (i >= page - windowSize && i <= page + windowSize)
            ) {
                pages.push(i);
            } else if (pages[pages.length - 1] !== "...") {
                pages.push("...");
            }
        }
        return pages;
    }, [page, totalPages]);

    const startIdx = (page - 1) * PAGE_SIZE + 1;
    const endIdx = Math.min(page * PAGE_SIZE, totalCount);

    if (isLoading) return <div className="p-3">Loading...</div>;

    return (
        <div className="container-fluid">
            <div className={`card border-0 shadow-sm ${styles.panel}`}>

                {/* Header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">Product Management</h5>
                        <small className="text-muted">
                            Quản lý danh sách sản phẩm
                        </small>
                    </div>

                    <Link to="/products/add" className={`btn ${styles.btnAccent}`}>
                        <i className="bi bi-plus-lg"></i> Add Product
                    </Link>
                </div>

                {/* Table */}
                <div className="table-responsive">
                    <table className="table align-middle mb-0">
                        <thead className="bg-light small text-uppercase text-muted">
                            <tr>
                                <th>ID</th>
                                <th>Product</th>
                                <th>SKU</th>
                                <th className="text-end">Qty</th>
                                <th className="text-end">Sold</th>
                                <th className="text-center">Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {products.map((p) => (
                                <tr key={p.id}>
                                    <td>{p.id}</td>

                                    <td>
                                        <div className="d-flex align-items-center gap-3">
                                            <img
                                                src={p.images?.find(img => img.is_primary)?.image_url}
                                                className={styles.productImg}
                                            />
                                            <div>
                                                <div className="fw-semibold">
                                                    {p.product_name}
                                                </div>
                                                <small className="text-muted">
                                                    {p.description}
                                                </small>
                                            </div>
                                        </div>
                                    </td>

                                    <td>
                                        <span className={styles.sku}>{p.sku}</span>
                                    </td>

                                    <td className="text-end">{p.total_quantity}</td>
                                    <td className="text-end">{p.total_sell}</td>

                                    <td className="text-center">
                                        <span
                                            className={`${styles.status} ${!p.is_deleted ? styles.active : styles.inactive}`}
                                        >
                                            {!p.isdeleted ? "Active" : "Inactive"}
                                        </span>
                                    </td>

                                    <td className="text-end">
                                        <Link
                                            to={`/products/${p.id}`}
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
                <div className="d-flex flex-wrap justify-content-between align-items-center mt-3 gap-2">

                    <small className="text-muted">
                        Hiển thị <strong>{startIdx}</strong>–<strong>{endIdx}</strong> trong{" "}
                        <strong>{totalCount}</strong> sản phẩm
                    </small>

                    <nav>
                        <ul className="pagination pagination-sm mb-0">

                            <li className={`page-item ${page === 1 ? "disabled" : ""}`}>
                                <button className="page-link" onClick={() => goTo(page - 1)}>
                                    ‹
                                </button>
                            </li>

                            {pageNumbers.map((n, idx) =>
                                n === "..." ? (
                                    <li key={idx} className="page-item disabled">
                                        <span className="page-link">…</span>
                                    </li>
                                ) : (
                                    <li
                                        key={n}
                                        className={`page-item ${page === n ? "active" : ""}`}
                                    >
                                        <button
                                            className="page-link"
                                            onClick={() => goTo(n)}
                                        >
                                            {n}
                                        </button>
                                    </li>
                                )
                            )}

                            <li className={`page-item ${page === totalPages ? "disabled" : ""}`}>
                                <button className="page-link" onClick={() => goTo(page + 1)}>
                                    ›
                                </button>
                            </li>

                        </ul>
                    </nav>

                </div>
            </div>
        </div>
    );
};

export default Products;