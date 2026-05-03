import { useState } from 'react';
import styles from './Categories.module.css';
import Pagination from '../components/Pagination';
import { useCategories } from '../hooks/categories/useCategory';
import CategoryRow from '../features/categories/components/CategoryRow';
import CategoryFormModal from '../features/categories/components/CategoryFormModal';

const PAGE_SIZE = 5;

const Categories = () => {
    const [page, setPage] = useState(1);
    const [expandedIds, setExpandedIds] = useState([]);
    const [status, setStatus] = useState("All");
    const [sort, setSort] = useState("Newest");

    const [modalState, setModalState] = useState({
        show: false,
        mode: 'create',
        initialValues: {},
    });

    const openCreate = () => setModalState({
        show: true,
        mode: 'create',
        initialValues: {},
    });

    const openUpdate = (node) => setModalState({
        show: true,
        mode: 'update',
        initialValues: {
            id: node.id,
            name: node.category_name,
            description: node.description,
            parentId: node.parent_category_id || '',
            isActive: !node.is_deleted,
        },
    });

    const closeModal = () => setModalState(prev => ({ ...prev, show: false }));

    const { categories, totalCount, isLoading, isError } = useCategories({
        PageNumber: page,
        PageSize: PAGE_SIZE,
        SortBy: sort,
        Status: status
    });


    const toggleExpand = (id) => {
        setExpandedIds((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
        );
    };

    if (isLoading) return <div className="p-3">Loading...</div>;
    if (isError) return <div className="p-3 text-danger">Không thể tải danh mục.</div>;

    return (
        <div className="container-fluid">
            <div className={`card border-0 shadow-sm ${styles.panel}`}>
                {/* Header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">Category Management</h5>
                        <small className="text-muted">Quản lý danh sách danh mục trong cửa hàng</small>
                    </div>
                    <button
                        className={`btn ${styles.btnAccent}`}
                        onClick={openCreate}
                    >
                        <i className="bi bi-plus-lg"></i> Add Category
                    </button>
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
                                <th style={{ width: '40px' }}></th>
                                <th>ID</th>
                                <th>Category</th>
                                <th className="text-center">Sub Categories</th>
                                <th className="text-center">Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {categories.map((cat, idx) => (
                                <CategoryRow
                                    key={cat.id}
                                    node={cat}
                                    depth={0}
                                    expandedIds={expandedIds}
                                    onToggle={toggleExpand}
                                    isLast={idx === categories.length - 1}
                                    ancestorIsLast={[]}
                                    onEdit={openUpdate}
                                />
                            ))}
                        </tbody>
                    </table>
                </div>

                <CategoryFormModal
                    {...modalState}
                    onHide={closeModal}
                    onSuccess={closeModal}
                />

                <Pagination
                    page={page}
                    totalCount={totalCount}
                    pageSize={PAGE_SIZE}
                    onPageChange={setPage}
                    itemLabel="danh mục"
                />
            </div>
        </div>
    );
};

export default Categories;