import { Fragment, useState } from 'react';
import styles from './Categories.module.css';

const mockCategories = [
    {
        id: '#1001',
        name: 'NAM',
        description: 'Thời trang dành cho nam giới',
        status: 'Active',
        subCategories: [
            { id: '#1001-1', name: 'Áo Nam', description: 'Áo thun, áo sơ mi, áo khoác', status: 'Active' },
            { id: '#1001-2', name: 'Quần Nam', description: 'Quần jean, quần tây, quần short', status: 'Active' },
            { id: '#1001-3', name: 'Phụ kiện Nam', description: 'Thắt lưng, ví, mũ', status: 'Inactive' },
        ],
    }
];

const Categories = () => {
    const [expandedIds, setExpandedIds] = useState([]);

    const toggleExpand = (id) => {
        setExpandedIds((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
        );
    };

    return (
        <div className="container-fluid">
            {/* Panel */}
            <div className={`card border-0 shadow-sm ${styles.panel}`}>
                {/* Header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">Category Management</h5>
                        <small className="text-muted">
                            Quản lý danh sách danh mục trong cửa hàng
                        </small>
                    </div>

                    <div>
                        <button className={`btn ${styles.btnAccent}`}>
                            <i className="bi bi-plus-lg"></i> Add Category
                        </button>
                    </div>
                </div>

                {/* Filters */}
                <div className="d-flex flex-wrap gap-2 mb-3">
                    <select className="form-select w-auto">
                        <option>All Status</option>
                        <option>Active</option>
                        <option>Inactive</option>
                    </select>

                    <select className="form-select w-auto">
                        <option>Sort by: Newest</option>
                        <option>Sort by: Oldest</option>
                        <option>Sort by: Name</option>
                    </select>
                </div>

                {/* Table */}
                <div className="table-responsive">
                    <table className="table align-middle mb-0">
                        <thead className="bg-light small text-uppercase text-muted">
                            <tr>
                                <th style={{ width: '40px' }}></th>
                                <th>ID</th>
                                <th>Category</th>
                                <th className="text-center">Sub Categories</th>
                                <th className="text-center">Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {mockCategories.map((cat) => {
                                const hasSubs = !!(cat.subCategories && cat.subCategories.length);
                                const isExpanded = expandedIds.includes(cat.id);

                                return (
                                    <Fragment key={cat.id}>
                                        {/* Parent row */}
                                        <tr className={hasSubs ? styles.parentRow : ''}>
                                            <td className="text-center">
                                                {hasSubs ? (
                                                    <button
                                                        className={styles.expandBtn}
                                                        onClick={() => toggleExpand(cat.id)}
                                                        aria-label="Toggle subcategories"
                                                    >
                                                        <i
                                                            className={`bi ${isExpanded
                                                                ? 'bi-chevron-down'
                                                                : 'bi-chevron-right'
                                                                }`}
                                                        ></i>
                                                    </button>
                                                ) : (
                                                    <span className={styles.dot}></span>
                                                )}
                                            </td>

                                            <td>{cat.id}</td>

                                            <td>
                                                <div className="d-flex align-items-center gap-3">
                                                    <div>
                                                        <div className="fw-semibold">{cat.name}</div>
                                                        <small className="text-muted">
                                                            {cat.description}
                                                        </small>
                                                    </div>
                                                </div>
                                            </td>

                                            <td className="text-center">
                                                {hasSubs ? (
                                                    <span className={styles.countBadge}>
                                                        {cat.subCategories.length}
                                                    </span>
                                                ) : (
                                                    <span className="text-muted">—</span>
                                                )}
                                            </td>

                                            <td className="text-center">
                                                <span
                                                    className={`${styles.status} ${cat.status === 'Active'
                                                        ? styles.active
                                                        : styles.inactive
                                                        }`}
                                                >
                                                    {cat.status}
                                                </span>
                                            </td>

                                            <td className="text-end">
                                                <button className={styles.actionBtn} title="View">
                                                    <i className="bi bi-eye"></i>
                                                </button>
                                                <button
                                                    className={styles.actionBtn}
                                                    title="Add Sub">
                                                    <i className="bi bi-plus-lg"></i>
                                                </button>
                                            </td>
                                        </tr>

                                        {/* SubCategory rows */}
                                        {hasSubs && isExpanded &&
                                            cat.subCategories.map((sub, idx) => {
                                                const isLast = idx === cat.subCategories.length - 1;
                                                return (
                                                    <tr key={sub.id} className={styles.subRow}>
                                                        <td></td>
                                                        <td>
                                                            <div className={styles.subIdWrap}>
                                                                <span
                                                                    className={`${styles.treeLine} ${isLast ? styles.treeLast : ''
                                                                        }`}
                                                                ></span>
                                                                <span className="text-muted small">
                                                                    {sub.id}
                                                                </span>
                                                            </div>
                                                        </td>
                                                        <td>
                                                            <div className="d-flex align-items-center gap-2">
                                                                <i
                                                                    className={`bi bi-arrow-return-right ${styles.subIcon}`}
                                                                ></i>
                                                                <div>
                                                                    <div className="fw-medium">
                                                                        {sub.name}
                                                                    </div>
                                                                    <small className="text-muted">
                                                                        {sub.description}
                                                                    </small>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center text-muted">
                                                            —
                                                        </td>
                                                        <td className="text-center">
                                                            <span
                                                                className={`${styles.status} ${sub.status === 'Active'
                                                                    ? styles.active
                                                                    : styles.inactive
                                                                    }`}
                                                            >
                                                                {sub.status}
                                                            </span>
                                                        </td>
                                                        <td className="text-end">
                                                            <button
                                                                className={styles.actionBtn}
                                                                title="View">
                                                                <i className="bi bi-eye"></i>
                                                            </button>

                                                            <button
                                                                className={styles.actionBtn}
                                                                title="Add Sub">
                                                                <i className="bi bi-plus-lg"></i>
                                                            </button>
                                                        </td>
                                                    </tr>
                                                );
                                            })}
                                    </Fragment>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default Categories;