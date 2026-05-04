import { Link } from 'react-router-dom';
import styles from './Sidebar.module.css';

const Sidebar = () => {
    const menuItems = [
        { id: 1, name: 'Dashboard', icon: 'bi-grid-fill', url: '/' },
        { id: 2, name: 'Products', icon: 'bi-box-seam', url: '/products' },
        { id: 3, name: 'Customers', icon: 'bi-people', url: '/users' },
        { id: 4, name: 'Category', icon: 'bi-tag', url: '/categories' },
        { id: 5, name: 'Orders', icon: 'bi-cart3' },
        { id: 6, name: 'Settings', icon: 'bi-gear' },
    ];

    return (
        <div className={`${styles.sidebar} d-flex flex-column p-3`}>
            <div className="mb-4">
                <div className={styles.logoWrapper}>
                    <img
                        src="https://res.cloudinary.com/novafashion/image/upload/v1776763238/novafashion_logo_uoa7wz.png"
                        alt="Logo"
                        className=""
                    />
                </div>
            </div>

            {/* Navigation */}
            <ul className="nav nav-pills flex-column mb-auto">
                {menuItems.map((item) => (
                    <li key={item.id} className="nav-item mb-1">

                        <Link to={item.url} className={`nav-link d-flex align-items-center justify-content-between ${item.active ? styles.activeLink : styles.link}`}>
                            <div className="d-flex align-items-center">
                                <i className={`bi ${item.icon} me-3`}></i>
                                {item.name}
                            </div>
                        </Link>

                    </li>
                ))}
            </ul>

        </div>
    );
};

export default Sidebar;