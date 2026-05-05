import { Link } from "react-router-dom";
import styles from "./Register.module.css";

const Register = () => {
    return (
        <div className={styles.wrapper}>
            <div className={styles.registerCard}>
                <h2 className={styles.title}>Create an Account</h2>
                <p className={styles.subtitle}>Register here if you are a new customer</p>

                <form>
                    <div className="row g-3 mb-3">
                        <div className="col-6">
                            <input
                                type="text"
                                name="firstName"
                                className={`form-control ${styles.input}`}
                                placeholder="First Name"
                            />
                        </div>
                        <div className="col-6">
                            <input
                                type="text"
                                name="lastName"
                                className={`form-control ${styles.input}`}
                                placeholder="Last Name"
                            />
                        </div>
                    </div>

                    <div className="mb-3">
                        <input
                            type="text"
                            name="username"
                            className={`form-control ${styles.input}`}
                            placeholder="Username"
                        />
                    </div>

                    <div className="mb-3">
                        <input
                            type="email"
                            name="email"
                            className={`form-control ${styles.input}`}
                            placeholder="Email Address"
                        />
                    </div>

                    <div className="mb-3">
                        <input
                            type="password"
                            name="password"
                            className={`form-control ${styles.input}`}
                            placeholder="Password"
                        />
                    </div>

                    <div className="mb-3">
                        <input
                            type="password"
                            name="confirmPassword"
                            className={`form-control ${styles.input}`}
                            placeholder="Confirm Password"
                        />
                    </div>

                    <button type="submit" className={`btn w-100 ${styles.btnRegister}`}>
                        Submit &amp; Register
                    </button>

                    <div className={`form-check mt-3 ${styles.terms}`}>
                        <input
                            className="form-check-input"
                            type="checkbox"
                            id="agree"
                        />
                        <label className="form-check-label" htmlFor="agree">
                            I have read and agree to the terms &amp; conditions
                        </label>
                    </div>

                    <span className="text-muted">Have account already? </span>
                    <Link to="/login" className={styles.loginLink}>Login now</Link>
                </form>
            </div>
        </div>
    );
};

export default Register;