import styles from './Products.module.css';

const Products = () => {
    return (
        <div className="container-fluid">
            {/* Panel */}
            <div className={`card border-0 shadow-sm ${styles.panel}`}>

                {/* Header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <h5 className="fw-bold mb-0">Product Management</h5>
                        <small className="text-muted">
                            Quản lý danh sách sản phẩm trong cửa hàng
                        </small>
                    </div>

                    <div>
                        <button className={`btn ${styles.btnAccent}`}>
                            <i className="bi bi-plus-lg"></i> Add Product
                        </button>
                    </div>
                </div>

                {/* Filters */}
                <div className="d-flex flex-wrap gap-2 mb-3">
                    <select className="form-select w-auto">
                        <option>All Categories</option>
                    </select>

                    <select className="form-select w-auto">
                        <option>All Status</option>
                    </select>

                    <select className="form-select w-auto">
                        <option>Sort by: Newest</option>
                    </select>
                </div>

                {/* Table */}
                <div className="table-responsive">
                    <table className="table align-middle mb-0">
                        <thead className="bg-light small text-uppercase text-muted">
                            <tr>
                                <th><input type="checkbox" /></th>
                                <th>ID</th>
                                <th>Product</th>
                                <th>SKU</th>
                                <th className="text-end">Qty</th>
                                <th className="text-end">Sold</th>
                                <th>Status</th>
                                <th className="text-end">Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            <tr>
                                <td><input type="checkbox" /></td>
                                <td>#1001</td>

                                <td>
                                    <div className="d-flex align-items-center gap-3">
                                        <img
                                            src="https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=100"
                                            className={styles.productImg}
                                        />
                                        <div>
                                            <div className="fw-semibold">Classic White Tee</div>
                                            <small className="text-muted">
                                                Áo thun cotton 100%
                                            </small>
                                        </div>
                                    </div>
                                </td>

                                <td><span className={styles.sku}>NF-WT-001</span></td>

                                <td className="text-end">120</td>
                                <td className="text-end">85</td>

                                <td>
                                    <span className={`${styles.status} ${styles.active}`}>
                                        Active
                                    </span>
                                </td>

                                <td className="text-end">
                                    <button className={styles.actionBtn}><i className="bi bi-eye"></i></button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

            </div>
        </div>
    );
};

export default Products;