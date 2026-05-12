import { Outlet, useLocation } from 'react-router-dom';
import Sidebar from '../components/Sidebar';
import Header from '../components/Header';

const routeTitles = {
    '/': 'Dashboard',
    '/products': 'Products',
    '/categories': 'Categories',
    '/orders': 'Orders'
};

export default function DashboardLayout() {
    const { pathname } = useLocation();

    return (
        <div className="container-fluid p-0">
            <div className="row g-0">

                <nav className="col-lg-2 d-none d-lg-block bg-light min-vh-100 border-end">
                    <Sidebar />
                </nav>


                <main className="col-lg-10 col-md-12 p-4 bg-light">
                    <Header title={routeTitles[pathname]} />

                    <Outlet />
                </main>
            </div>
        </div>
    );
}