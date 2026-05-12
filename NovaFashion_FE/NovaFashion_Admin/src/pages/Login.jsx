import { useLogin } from "../hooks/authentications/useLogin";
import styles from "./Login.module.css";

const Login = () => {
    const { handleSubmit, error, loading } = useLogin();

    return (
        <div className={styles.wrapper}>
            <div className={styles.card}>
                <h2 className={styles.title}>Login</h2>
                <p className={styles.subtitle}>
                    Welcome back administrator.
                </p>

                {error && (
                    <div className="alert alert-danger py-2" role="alert">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <input
                            type="email"
                            name="email"
                            className={`form-control ${styles.input}`}
                            placeholder="Email Address"
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <input
                            type="password"
                            name="password"
                            className={`form-control ${styles.input}`}
                            placeholder="Password"
                            required
                        />
                    </div>

                    <div className={`d-flex justify-content-between align-items-center mb-4`}>
                        <div className="form-check mb-0">
                            <input
                                className="form-check-input"
                                type="checkbox"
                                name="remember"
                                id="remember"
                            />
                            <label className="form-check-label" htmlFor="remember">
                                Remember me
                            </label>
                        </div>
                        <a href="#" className={styles.forgot}>
                            Forgot Your Password?
                        </a>
                    </div>

                    <button
                        type="submit"
                        className={`btn w-100 ${styles.btnLogin}`}
                        disabled={loading}
                    >
                        {loading ? "Logging in..." : "Login"}
                    </button>

                </form>
            </div>
        </div>
    );
};

export default Login;