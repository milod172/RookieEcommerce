import { Route, Routes } from 'react-router-dom';
import DashboardLayout from '../layouts/DashboardLayout';
import Dashboard from '../pages/Dashboard';
import Products from '../pages/Products';
import Categories from '../pages/Categories';
import './index.module.css'
import AddProduct from '../pages/AddProduct';
import ProductDetails from '../pages/ProductDetails';

const AppRoutes = () => {
    return (
        <Routes>
            <Route element={<DashboardLayout />}>
                <Route path="/" element={<Dashboard />} />

                <Route path="/products">
                    <Route index element={<Products />} />
                    <Route path="add" element={<AddProduct />} />
                    <Route path="view" element={<ProductDetails />} />
                </Route>

                <Route path="/categories" element={<Categories />} />
            </Route>
        </Routes>
    );
};
export default AppRoutes;