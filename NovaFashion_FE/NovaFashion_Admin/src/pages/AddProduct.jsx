import { useState, useRef, useMemo } from 'react';
import styles from './AddProduct.module.css';
import { Link } from 'react-router-dom';

// Mock data — sau này thay bằng API
const CATEGORIES = [
    {
        id: 'cat-1',
        name: 'NAM',
        subCategories: [
            { id: 'sub-1-1', name: 'Áo thun nam' },
            { id: 'sub-1-2', name: 'Quần jeans nam' },
            { id: 'sub-1-3', name: 'Áo khoác nam' },
        ],
    },
    {
        id: 'cat-2',
        name: 'NỮ',
        subCategories: [
            { id: 'sub-2-1', name: 'Đầm nữ' },
            { id: 'sub-2-2', name: 'Áo sơ mi nữ' },
        ],
    },
    {
        id: 'cat-3',
        name: 'PHỤ KIỆN',
        subCategories: [],
    },
];

const AddProduct = () => {
    const fileInputRef = useRef(null);

    const [form, setForm] = useState({
        name: '',
        description: '',
        details: '',
        totalQuantity: '',
        basePrice: '',
        categoryId: '',
        subCategoryId: '',
        status: 'active',
    });

    const [images, setImages] = useState([]); // [{ id, url, file, name }]

    const subCategories = useMemo(() => {
        const cat = CATEGORIES.find((c) => c.id === form.categoryId);
        return cat?.subCategories ?? [];
    }, [form.categoryId]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleCategoryChange = (e) => {
        setForm((prev) => ({
            ...prev,
            categoryId: e.target.value,
            subCategoryId: '', // reset sub khi đổi category
        }));
    };

    const handleFiles = (fileList) => {
        const files = Array.from(fileList || []);
        const newItems = files.map((file) => ({
            id: `${file.name}-${file.size}-${Date.now()}-${Math.random()}`,
            url: URL.createObjectURL(file),
            name: file.name,
            file,
        }));
        setImages((prev) => [...prev, ...newItems]);
    };

    const handleRemoveImage = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (target) URL.revokeObjectURL(target.url);
            return prev.filter((i) => i.id !== id);
        });
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (e.dataTransfer?.files?.length) {
            handleFiles(e.dataTransfer.files);
        }
    };

    const handleDragOver = (e) => {
        e.preventDefault();
        e.stopPropagation();
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        const payload = { ...form, images };
        console.log('Submit product:', payload);
    };

    const handleReset = () => {
        images.forEach((img) => URL.revokeObjectURL(img.url));
        setImages([]);
        setForm({
            name: '',
            description: '',
            details: '',
            totalQuantity: '',
            basePrice: '',
            categoryId: '',
            subCategoryId: '',
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
                            <Link to="/products" className={styles.backDecorationNone}><span>Products</span></Link>
                            <i className="bi bi-chevron-right" style={{ fontSize: '0.7rem' }}></i>
                            <span className="text-dark">Add new</span>
                        </div>
                        <h5 className="fw-bold mb-0">Add New Product</h5>
                        <small className="text-muted">
                            Tạo sản phẩm mới cho cửa hàng của bạn
                        </small>
                    </div>

                    <div className="d-flex gap-2">

                        <button
                            type="button"
                            className="btn btn-light border"
                            onClick={handleReset}
                        >
                            Cancel
                        </button>
                        <button type="submit" className={`btn ${styles.btnAccent}`}>
                            <i className="bi bi-check2"></i> Save Product
                        </button>
                    </div>
                </div>

                <div className="row g-3">
                    {/* LEFT — Main info */}
                    <div className="col-lg-8">
                        {/* General info */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>General Information</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">
                                    Product Name <span className="text-danger">*</span>
                                </label>
                                <input
                                    type="text"
                                    name="name"
                                    value={form.name}
                                    onChange={handleChange}
                                    className="form-control"
                                    placeholder="VD: Áo thun cotton basic"
                                    required
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">
                                    Description
                                </label>
                                <textarea
                                    name="description"
                                    value={form.description}
                                    onChange={handleChange}
                                    className="form-control"
                                    rows={3}
                                    placeholder="Mô tả ngắn gọn về sản phẩm..."
                                />
                            </div>

                            <div className="mb-0">
                                <label className="form-label small fw-semibold">Details</label>
                                <textarea
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
                                <h6 className={`${styles.sectionTitle} mb-0`}>
                                    Product Images
                                </h6>
                                <small className="text-muted">
                                    {images.length} ảnh đã thêm
                                </small>
                            </div>

                            <div
                                className={styles.dropzone}
                                onDrop={handleDrop}
                                onDragOver={handleDragOver}
                                onClick={() => fileInputRef.current?.click()}
                            >
                                <i className="bi bi-cloud-arrow-up" style={{ fontSize: '1.75rem' }}></i>
                                <div className="fw-semibold mt-2">
                                    Kéo & thả ảnh vào đây
                                </div>
                                <small className="text-muted">
                                    hoặc <span className={styles.linkAccent}>chọn từ máy tính</span> ·
                                    PNG, JPG, WEBP
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
                        {/* Pricing & inventory */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Pricing & Inventory</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">
                                    Base Price <span className="text-danger">*</span>
                                </label>
                                <div className="input-group">
                                    <span className="input-group-text">₫</span>
                                    <input
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
                                <label className="form-label small fw-semibold">
                                    Total Quantity <span className="text-danger">*</span>
                                </label>
                                <input
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

                        {/* Cascading category */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Categorization</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">
                                    Category <span className="text-danger">*</span>
                                </label>
                                <select
                                    name="categoryId"
                                    value={form.categoryId}
                                    onChange={handleCategoryChange}
                                    className="form-select"
                                    required
                                >
                                    <option value="">-- Chọn category --</option>
                                    {CATEGORIES.map((c) => (
                                        <option key={c.id} value={c.id}>
                                            {c.name}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="mb-0">
                                <label className="form-label small fw-semibold">
                                    Sub Category
                                </label>
                                <select
                                    name="subCategoryId"
                                    value={form.subCategoryId}
                                    onChange={handleChange}
                                    className="form-select"
                                    disabled={!form.categoryId || subCategories.length === 0}
                                >
                                    <option value="">
                                        {!form.categoryId
                                            ? '-- Chọn category trước --'
                                            : subCategories.length === 0
                                                ? '-- Không có sub category --'
                                                : '-- Chọn sub category --'}
                                    </option>
                                    {subCategories.map((s) => (
                                        <option key={s.id} value={s.id}>
                                            {s.name}
                                        </option>
                                    ))}
                                </select>

                                {form.categoryId && (
                                    <div className={styles.breadcrumbPath}>
                                        <i className="bi bi-diagram-3"></i>
                                        <span>
                                            {CATEGORIES.find((c) => c.id === form.categoryId)?.name}
                                        </span>
                                        {form.subCategoryId && (
                                            <>
                                                <i className="bi bi-chevron-right" style={{ fontSize: '0.65rem' }}></i>
                                                <span className="fw-semibold">
                                                    {
                                                        subCategories.find((s) => s.id === form.subCategoryId)
                                                            ?.name
                                                    }
                                                </span>
                                            </>
                                        )}
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Status */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Status</h6>
                            <select
                                name="status"
                                value={form.status}
                                onChange={handleChange}
                                className="form-select"
                            >
                                <option value="active">Active</option>
                                <option value="inactive">Inactive</option>
                            </select>
                            <small className="text-muted d-block mt-2">
                                Sản phẩm "Inactive" sẽ không hiển thị ngoài cửa hàng.
                            </small>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    );
};

export default AddProduct;
