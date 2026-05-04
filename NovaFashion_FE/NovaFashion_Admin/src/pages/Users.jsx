import { Link } from 'react-router-dom';
import styles from './Users.module.css';
import { useState } from 'react';
import Pagination from '../components/Pagination.jsx';
import { useUsers } from '../hooks/users/useUsers.js';

const PAGE_SIZE = 5;

const Users = () => {
    const [page, setPage] = useState(1);
    const [status, setStatus] = useState("All");
    const [sort, setSort] = useState("Newest");

    const { users, totalCount, isLoading } = useUsers({
        PageNumber: page,
        PageSize: PAGE_SIZE,
        SortBy: sort,
        Status: status,
    });

    if (isLoading) return <div className="p-3">Loading...</div>;

    return (
        <div className="container-fluid">
            <div className={`card border-0 shadow-sm ${styles.panel}`}>

                {/* Header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">User Management</h5>
                        <small className="text-muted">
                            Quản lý danh sách người dùng
                        </small>
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
                        <option value="Active">Active</option>
                        <option value="Inactive">Inactive</option>
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
                        <option value="NameAsc">Sort by: Tên từ A - Z</option>
                        <option value="NameDesc">Sort by: Tên từ Z - A</option>
                        <option value="IdAsc">Sort by: Id tăng dần</option>
                        <option value="IdDesc">Sort by: Id giảm dần</option>
                    </select>
                </div>

                {/* Table */}
                <div className="table-responsive">
                    <table className="table align-middle mb-0">
                        <thead className="bg-light small text-uppercase text-muted">
                            <tr>
                                <th>ID</th>
                                <th>User name</th>
                                <th>Full name</th>
                                <th className="text-start">Email</th>
                                <th className="text-end">Phone</th>
                                <th className="text-center">Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {users.map((p) => (
                                <tr key={p.id}>
                                    <td>{p.id.slice(0, 8)}</td>

                                    <td>
                                        <div className="d-flex align-items-center gap-3">
                                            <div>
                                                <div className="fw-semibold">
                                                    {p.user_name}
                                                </div>
                                            </div>
                                        </div>
                                    </td>

                                    <td>{p.full_name}</td>
                                    <td>
                                        <span>{p.email}</span>
                                    </td>

                                    <td className="text-end">{p.phone_number}</td>

                                    <td className="text-center">
                                        <span
                                            className={`${styles.status} ${p.is_active ? styles.active : styles.inactive}`}
                                        >
                                            {p.is_active ? "Active" : "Inactive"}
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
                <Pagination
                    page={page}
                    totalCount={totalCount}
                    pageSize={PAGE_SIZE}
                    onPageChange={setPage}
                    itemLabel="người dùng"
                />
            </div>
        </div>
    );
};

export default Users;