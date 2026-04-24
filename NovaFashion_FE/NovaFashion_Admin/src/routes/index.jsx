
import { Route, Routes } from 'react-router-dom';
import Dashboard from '../pages/Dashboard';
import Products from '../pages/Products';
import './index.module.css'
import DashboardLayout from '../layouts/DashboardLayout';

const AppRoutes = () => {
    return (
        <Routes>
            <Route element={<DashboardLayout />}>
                <Route path="/" element={<Dashboard />} />
                <Route path="/products" element={<Products />} />
            </Route>
        </Routes>
    );
};
export default AppRoutes;