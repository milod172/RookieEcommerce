import StatCard from '../components/StatCard.jsx';
import OrderTable from '../components/OrderTable.jsx';

const Dashboard = () => {
    return (
        <>
            <div className="row g-3 mb-4">
                <div className="col-12 col-sm-6 col-xl-3">
                    <StatCard title="Total Sales" value="2680" icon="💰" />
                </div>
                <div className="col-12 col-sm-6 col-xl-3">
                    <StatCard title="New Customers" value="537" icon="👤" />
                </div>
                <div className="col-12 col-sm-6 col-xl-3">
                    <StatCard title="Product Sold" value="846" icon="📦" />
                </div>
                <div className="col-12 col-sm-6 col-xl-3">
                    <StatCard title="Total Revenue" value="$783.83" icon="💵" />
                </div>
            </div>


            <div className="row g-3 mb-4">
                <div className="col-12 col-xl-8">
                    <div className="card border-0 shadow-sm p-3">
                        <h5>Revenue analytics</h5>
                        {/* Chart component here */}
                    </div>
                </div>
                <div className="col-12 col-xl-4">
                    <div className="card border-0 shadow-sm p-3">
                        <h5>Sales by traffic source</h5>
                        {/* Traffic component here */}
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-12">
                    <OrderTable />
                </div>
            </div>
        </>
    );
};

export default Dashboard;