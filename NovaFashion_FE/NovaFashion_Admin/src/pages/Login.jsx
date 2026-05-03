import { useLogin } from "../hooks/authentications/useLogin";
import styles from "./Login.module.css";

const Login = () => {
    const { handleSubmit, error, loading } = useLogin();
    return (
        <div
            className={`d-flex align-items-center justify-content-center min-vh-100 ${styles.wrapper}`}
        >
            <div
                className={`card shadow-lg border-0 ${styles.card}`}
            >
                <div className="card-body p-4 p-sm-5">
                    <h2 className="card-title text-center mb-1 fw-bold">Đăng nhập</h2>
                    <p className="text-center text-muted mb-4">
                        Chào mừng bạn quay trở lại
                    </p>

                    {error && (
                        <div className="alert alert-danger py-2" role="alert">
                            {error}
                        </div>
                    )}

                    <form onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <label htmlFor="email" className="form-label fw-semibold">
                                Email
                            </label>
                            <input
                                type="email"
                                className="form-control form-control-lg"
                                id="email"
                                name="email"
                                placeholder="name@example.com"
                                required
                            />
                        </div>

                        <div className="mb-4">
                            <label htmlFor="password" className="form-label fw-semibold">
                                Mật khẩu
                            </label>
                            <input
                                type="password"
                                className="form-control form-control-lg"
                                id="password"
                                name="password"
                                placeholder="••••••••"
                                required
                            />
                        </div>

                        <button
                            type="submit"
                            className="btn btn-primary btn-lg w-100 mb-3 fw-semibold"
                            disabled={loading}
                        >
                            {loading ? "Đang đăng nhập..." : "Đăng nhập"}
                        </button>

                        <div className="text-center">
                            <span className="text-muted">Chưa có tài khoản? </span>
                            <button
                                type="button"
                                className="btn btn-link p-0 fw-semibold text-decoration-none"
                                onClick={() => console.log("Chuyển sang đăng ký")}
                            >
                                Đăng ký
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default Login;