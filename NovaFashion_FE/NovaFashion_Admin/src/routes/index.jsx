import { Navigate, Route, Routes } from 'react-router-dom';
import DashboardLayout from '../layouts/DashboardLayout';
import Dashboard from '../pages/Dashboard';
import Products from '../pages/Products';
import Categories from '../pages/Categories';
import Login from '../pages/Login';
import './index.module.css'
import CreateProduct from '../pages/CreateProduct';
import ProductDetails from '../pages/ProductDetails';
import Users from '../pages/Users';
import Register from '../pages/Register';
import Orders from '../pages/Orders';
import OrderDetails from '../pages/OrderDetails';
import NotFound from '../pages/NotFound';


const ProtectedRoute = ({ children }) => {
    const accessToken = localStorage.getItem('access_token');
    const refreshToken = localStorage.getItem('refresh_token');

    if (!accessToken || !refreshToken) {
        return <Navigate to="/login" replace />;
    }
    return children;
};

const AppRoutes = () => {
    return (
        <Routes>
            <Route element={
                <ProtectedRoute>
                    <DashboardLayout />
                </ProtectedRoute>
            }>
                <Route path="/" element={<Dashboard />} />

                <Route path="/products">
                    <Route index element={<Products />} />
                    <Route path="add" element={<CreateProduct />} />
                    <Route path=":id" element={<ProductDetails />} />
                </Route>

                <Route path="/users" element={<Users />} />
                <Route path="/categories" element={<Categories />} />
                <Route path="/orders">
                    <Route index element={<Orders />} />
                    <Route path=":id" element={<OrderDetails />} />
                </Route>

            </Route>

            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
};
export default AppRoutes;