import styles from './OrderTable.module.css';

const orders = [
    { id: '#7480', brand: 'Adidas', date: '3/12/25', deal: 'Bessie Cooper', category: 'Streetwear', items: 8, total: '$623.0', stage: 'Done' },
    { id: '#7481', brand: 'Nike', date: '4/12/25', deal: 'Arlene McCoy', category: 'Shoes', items: 16, total: '$804.0', stage: 'In Progress' },
    { id: '#7488', brand: 'Uniqlo', date: '5/12/25', deal: 'Devon Lane', category: 'Crewncek', items: 10, total: '$550.0', stage: 'Canceled' },
];

const OrderTable = () => (
    <div className="card border-0 shadow-sm p-4 rounded-4">
        {/* Header Table: Dùng Bootstrap Flexbox để responsive */}
        <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
            <h3 className="fw-bold mb-0">Recent orders</h3>
            <div className="d-flex gap-2">
                <div className="input-group input-group-sm" style={{ maxWidth: '250px' }}>
                    <span className="input-group-text bg-white border-end-0">
                        <i className="bi bi-search text-muted"></i>
                    </span>
                    <input
                        type="text"
                        className="form-control border-start-0"
                        placeholder="Search"
                    />
                </div>
                <button className="btn btn-light border btn-sm d-flex align-items-center gap-2">
                    <i className="bi bi-filter"></i> Filter
                </button>
            </div>
        </div>

        {/* Bọc trong table-responsive để không bị vỡ giao diện trên Mobile */}
        <div className="table-responsive">
            <table className={`table align-middle ${styles.customTable}`}>
                <thead className="table-light">
                    <tr className="text-muted small">
                        <th><input type="checkbox" className="form-check-input" /> Order Id</th>
                        <th>Brands</th>
                        <th>Activities</th>
                        <th>Name of deals</th>
                        <th>Category</th>
                        <th>Items</th>
                        <th>Total</th>
                        <th>Stage</th>
                        <th className="text-center">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {orders.map((order) => (
                        <tr key={order.id}>
                            <td className="fw-medium">
                                <input type="checkbox" className="form-check-input me-2" /> {order.id}
                            </td>
                            <td>{order.brand}</td>
                            <td className="text-muted">{order.date}</td>
                            <td className="fw-semibold text-dark">{order.deal}</td>
                            <td>{order.category}</td>
                            <td>{order.items}</td>
                            <td className="fw-bold">{order.total}</td>
                            <td>
                                <span className={`badge ${styles.badgeStatus} ${styles[order.stage.toLowerCase().replace(' ', '')]}`}>
                                    {order.stage === 'In Progress' ? (
                                        <i className="bi bi-brightness-high me-1"></i>
                                    ) : order.stage === 'Done' ? (
                                        <i className="bi bi-check2 me-1"></i>
                                    ) : (
                                        <i className="bi bi-x-lg me-1"></i>
                                    )}
                                    {order.stage}
                                </span>
                            </td>
                            <td className="text-center text-muted">
                                <i className="bi bi-three-dots cursor-pointer"></i>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    </div>
);

export default OrderTable;