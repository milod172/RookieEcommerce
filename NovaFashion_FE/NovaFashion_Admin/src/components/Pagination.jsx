// components/common/Pagination.jsx
import { usePagination } from "../hooks/usePagination.js";

const Pagination = ({ page, totalCount, pageSize, onPageChange, itemLabel = "mục" }) => {
    const { totalPages, pageNumbers, startIdx, endIdx } = usePagination({
        page,
        totalCount,
        pageSize,
    });

    const goTo = (p) => {
        const next = Math.min(Math.max(1, p), totalPages);
        onPageChange(next);
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    return (
        <div className="d-flex flex-wrap justify-content-between align-items-center mt-3 gap-2">
            <small className="text-muted">
                Hiển thị <strong>{startIdx}</strong>–<strong>{endIdx}</strong> trong{" "}
                <strong>{totalCount}</strong> {itemLabel}
            </small>

            <nav>
                <ul className="pagination pagination-sm mb-0">
                    <li className={`page-item ${page === 1 ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => goTo(page - 1)}>‹</button>
                    </li>

                    {pageNumbers.map((n, idx) =>
                        n === "..." ? (
                            <li key={idx} className="page-item disabled">
                                <span className="page-link">…</span>
                            </li>
                        ) : (
                            <li key={n} className={`page-item ${page === n ? "active" : ""}`}>
                                <button className="page-link" onClick={() => goTo(n)}>{n}</button>
                            </li>
                        )
                    )}

                    <li className={`page-item ${page === totalPages ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => goTo(page + 1)}>›</button>
                    </li>
                </ul>
            </nav>
        </div>
    );
};

export default Pagination;