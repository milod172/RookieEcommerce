import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authApi } from '../../features/authentications/authApi';

export const useLogin = () => {
    const navigate = useNavigate();
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        const formData = new FormData(e.target);
        const credentials = {
            email: formData.get("email"),
            password: formData.get("password"),
        };

        try {
            await authApi.login(credentials);
            if (!authApi.isAdmin()) {
                authApi.logout();
                setError("Bạn không có quyền truy cập trang này");
                return;
            }
            navigate("/");

        } catch (err) {
            setError(
                err.response?.data?.errors?.[0]?.message
                ?? "Email hoặc mật khẩu không đúng"
            );

        } finally {
            setLoading(false);
        }
    };

    return { handleSubmit, error, loading };

};