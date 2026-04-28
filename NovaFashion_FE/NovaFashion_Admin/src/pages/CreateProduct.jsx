import { useState, useRef } from 'react';
import styles from './CreateProduct.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useCreateProduct } from '../hooks/products/useCreateProduct.js';
import { useCategoryTree } from '../hooks/products/useCategoryTree.js';
import { useProductImages } from '../hooks/products/useProductImages.js';
import { useCategories } from '../hooks/categories/useCategory.js';
import CategorySection from '../features/categories/components/CategorySection.jsx';

const CreateProduct = () => {
    const navigate = useNavigate();
    const fileInputRef = useRef(null);

    const { createProduct, isCreating } = useCreateProduct();
    const { categories, isLoading: categoriesLoading, isError: categoriesError } = useCategories({
        pageNumber: 1,
        pageSize: 20
    });

    // ===== FORM =====
    const [form, setForm] = useState({
        name: '',
        description: '',
        details: '',
        totalQuantity: '',
        basePrice: '',
        status: 'active',
    });

    const [selectedPath, setSelectedPath] = useState([]);
    const [error, setError] = useState(null);

    // ===== HOOKS =====
    const {
        categoryDropdowns,
        finalCategoryId,
        breadcrumbPath,
        handleCategorySelect
    } = useCategoryTree(categories, selectedPath, setSelectedPath);

    const {
        images,
        handleFiles,
        handleRemoveImage,
        handleDrop,
        handleDragOver,
        clearImages
    } = useProductImages();

    // FORM CHANGE
    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    // SUBMIT
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);

        try {
            const formPayload = {
                product_name: form.name,
                description: form.description,
                unit_price: Number(form.basePrice),
                details: form.details,
                total_quantity: Number(form.totalQuantity),
                category_id: finalCategoryId,
            };

            await createProduct({
                form: formPayload,
                images: images,
            });

            navigate('/products');

        } catch (err) {
            setError(err?.response?.data?.message || 'Có lỗi xảy ra');
        }
    };

    // RESET
    const handleReset = () => {
        clearImages();
        setSelectedPath([]);
        setForm({
            name: '',
            description: '',
            details: '',
            totalQuantity: '',
            basePrice: '',
            status: 'active',
        });
    };

    return (
        <div className="container-fluid">
            <form onSubmit={handleSubmit}>
                {/* Top header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <div className="d-flex align-items-center gap-2 text-muted small mb-1">
                            <i className="bi bi-box-seam"></i>
                            <Link to="/products" className={styles.backDecorationNone}>
                                <span>Products</span>
                            </Link>
                            <i className="bi bi-chevron-right" style={{ fontSize: '0.7rem' }}></i>
                            <span className="text-dark">Add new</span>
                        </div>
                        <h5 className="fw-bold mb-0">Add New Product</h5>
                        <small className="text-muted">Tạo sản phẩm mới cho cửa hàng của bạn</small>
                    </div>

                    <div className="d-flex gap-2">
                        <button
                            type="button"
                            className="btn btn-light border"
                            onClick={handleReset}
                            disabled={isCreating}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className={`btn ${styles.btnAccent}`}
                            disabled={isCreating}
                        >
                            {isCreating ? (
                                <>
                                    <span
                                        className="spinner-border spinner-border-sm me-1"
                                        role="status"
                                        aria-hidden="true"
                                    />
                                    Saving...
                                </>
                            ) : (
                                <>
                                    <i className="bi bi-check2"></i> Save Product
                                </>
                            )}
                        </button>
                    </div>
                </div>

                {/* Error alert */}
                {error && (
                    <div className="alert alert-danger d-flex align-items-center gap-2 mb-3" role="alert">
                        <i className="bi bi-exclamation-triangle-fill"></i>
                        <span>{error}</span>
                    </div>
                )}

                <div className="row g-3">
                    {/* LEFT — Main info */}
                    <div className="col-lg-8">
                        {/* General info */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>General Information</h6>

                            <div className="mb-3">
                                <label htmlFor="name" className="form-label small fw-semibold">
                                    Product Name <span className="text-danger">*</span>
                                </label>
                                <input
                                    type="text"
                                    id="name"
                                    name="name"
                                    value={form.name}
                                    onChange={handleChange}
                                    className="form-control"
                                    placeholder="VD: Áo thun cotton basic"
                                    required
                                />
                            </div>

                            <div className="mb-3">
                                <label htmlFor="description" className="form-label small fw-semibold">
                                    Description
                                </label>
                                <textarea
                                    id="description"
                                    name="description"
                                    value={form.description}
                                    onChange={handleChange}
                                    className="form-control"
                                    rows={3}
                                    placeholder="Mô tả ngắn gọn về sản phẩm..."
                                />
                            </div>

                            <div className="mb-0">
                                <label htmlFor="details" className="form-label small fw-semibold">
                                    Details
                                </label>
                                <textarea
                                    id="details"
                                    name="details"
                                    value={form.details}
                                    onChange={handleChange}
                                    className="form-control"
                                    rows={5}
                                    placeholder="Chất liệu, kích thước, hướng dẫn bảo quản..."
                                />
                                <small className="text-muted">
                                    Thông tin chi tiết hiển thị ở tab "Chi tiết sản phẩm".
                                </small>
                            </div>
                        </div>

                        {/* Images */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <div className="d-flex justify-content-between align-items-center mb-2">
                                <h6 className={`${styles.sectionTitle} mb-0`}>Product Images</h6>
                                <small className="text-muted">{images.length} ảnh đã thêm</small>
                            </div>

                            <div
                                className={styles.dropzone}
                                onDrop={handleDrop}
                                onDragOver={handleDragOver}
                                onClick={() => fileInputRef.current?.click()}
                                onKeyDown={(e) => {
                                    if (e.key === 'Enter' || e.key === ' ') {
                                        fileInputRef.current?.click();
                                    }
                                }}
                                role="button"
                                tabIndex={0}
                                aria-label="Upload product images"
                            >
                                <i className="bi bi-cloud-arrow-up" style={{ fontSize: '1.75rem' }}></i>
                                <div className="fw-semibold mt-2">Kéo & thả ảnh vào đây</div>
                                <small className="text-muted">
                                    hoặc{' '}
                                    <span className={styles.linkAccent}>chọn từ máy tính</span> · PNG, JPG, WEBP
                                </small>
                                <input
                                    ref={fileInputRef}
                                    type="file"
                                    accept="image/*"
                                    multiple
                                    hidden
                                    onChange={(e) => handleFiles(e.target.files)}
                                />
                            </div>

                            {images.length > 0 && (
                                <div className={styles.imageGrid}>
                                    {images.map((img, idx) => (
                                        <div key={img.id} className={styles.imageItem}>
                                            <img src={img.url} alt={img.name} />
                                            {idx === 0 && (
                                                <span className={styles.coverBadge}>Cover</span>
                                            )}
                                            <button
                                                type="button"
                                                className={styles.removeBtn}
                                                onClick={() => handleRemoveImage(img.id)}
                                                aria-label="Remove image"
                                            >
                                                <i className="bi bi-x-lg"></i>
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                    </div>

                    {/* RIGHT — Side panels */}
                    <div className="col-lg-4">
                        {/* Pricing & Inventory */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Pricing & Inventory</h6>

                            <div className="mb-3">
                                <label htmlFor="baseprice" className="form-label small fw-semibold">
                                    Base Price <span className="text-danger">*</span>
                                </label>
                                <div className="input-group">
                                    <span className="input-group-text">₫</span>
                                    <input
                                        id="baseprice"
                                        type="number"
                                        name="basePrice"
                                        value={form.basePrice}
                                        onChange={handleChange}
                                        className="form-control"
                                        placeholder="0"
                                        min="0"
                                        required
                                    />
                                </div>
                            </div>

                            <div className="mb-0">
                                <label htmlFor="totalQuantity" className="form-label small fw-semibold">
                                    Total Quantity <span className="text-danger">*</span>
                                </label>
                                <input
                                    id="totalQuantity"
                                    type="number"
                                    name="totalQuantity"
                                    value={form.totalQuantity}
                                    onChange={handleChange}
                                    className="form-control"
                                    placeholder="0"
                                    min="0"
                                    required
                                />
                            </div>
                        </div>

                        {/* Categorization */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Categorization</h6>

                            <CategorySection
                                isLoading={categoriesLoading}
                                isError={categoriesError}
                                categoryDropdowns={categoryDropdowns}
                                breadcrumbPath={breadcrumbPath}
                                handleCategorySelect={handleCategorySelect}
                            />
                        </div>

                        {/* Status */}
                    </div>
                </div>
            </form>
        </div>
    );
};

export default CreateProduct;