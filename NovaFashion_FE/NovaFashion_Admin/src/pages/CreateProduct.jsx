import { useState, useRef, useMemo } from 'react';
import styles from './CreateProduct.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useCreateProduct } from '../hooks/products/useCreateProduct.js';

// Mock data — sau này thay bằng API
const CATEGORIES = [
    {
        id: 'cat-1',
        name: 'NAM',
        subCategories: [
            {
                id: 'sub-1-1',
                name: 'Áo thun nam',
                subCategories: [
                    { id: 'sub-1-1-1', name: 'Áo thun tay ngắn', subCategories: [] },
                    { id: 'sub-1-1-2', name: 'Áo thun tay dài', subCategories: [] },
                ],
            },
            { id: 'sub-1-2', name: 'Quần jeans nam', subCategories: [] },
            { id: 'sub-1-3', name: 'Áo khoác nam', subCategories: [] },
        ],
    },
    {
        id: 'cat-2',
        name: 'NỮ',
        subCategories: [
            { id: 'sub-2-1', name: 'Đầm nữ', subCategories: [] },
            { id: 'sub-2-2', name: 'Áo sơ mi nữ', subCategories: [] },
        ],
    },
    {
        id: 'cat-3',
        name: 'PHỤ KIỆN',
        subCategories: [],
    },
];

/*Tìm node bất kỳ trong cây category theo id*/
const findNode = (nodes, id) => {
    for (const node of nodes) {
        if (node.id === id) return node;
        if (node.subCategories?.length) {
            const found = findNode(node.subCategories, id);
            if (found) return found;
        }
    }
    return null;
};

/*Build đường dẫn breadcrumb từ root → node được chọn*/
const buildPath = (nodes, targetId, path = []) => {
    for (const node of nodes) {
        const current = [...path, node];
        if (node.id === targetId) return current;
        if (node.subCategories?.length) {
            const found = buildPath(node.subCategories, targetId, current);
            if (found) return found;
        }
    }
    return null;
};

const CreateProduct = () => {
    const fileInputRef = useRef(null);
    const navigate = useNavigate();
    const { createProduct } = useCreateProduct();

    const [form, setForm] = useState({
        name: '',
        description: '',
        details: '',
        totalQuantity: '',
        basePrice: '',
        status: 'active',
    });


    const [selectedPath, setSelectedPath] = useState([]);
    const [images, setImages] = useState([]);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);

    // -------- Category helpers --------

    /**
     * Trả về danh sách các dropdowns cần hiển thị:
     * - Luôn có dropdown cấp 0 (root categories)
     * - Nếu cấp i đã chọn và node đó có subCategories → hiển thị dropdown cấp i+1
     */
    const categoryDropdowns = useMemo(() => {
        const dropdowns = [{ level: 0, options: CATEGORIES, selectedId: selectedPath[0] ?? '' }];

        for (let i = 0; i < selectedPath.length; i++) {
            const node = findNode(CATEGORIES, selectedPath[i]);
            if (node?.subCategories?.length > 0) {
                dropdowns.push({
                    level: i + 1,
                    options: node.subCategories,
                    selectedId: selectedPath[i + 1] ?? '',
                });
            } else {
                break;
            }
        }

        return dropdowns;
    }, [selectedPath]);

    /**
     * ID category cuối cùng được chọn (deepest selected node)
     */
    const finalCategoryId = selectedPath.length > 0 ? selectedPath[selectedPath.length - 1] : null;

    /**
     * Breadcrumb text của path đã chọn
     */
    const breadcrumbPath = useMemo(() => {
        if (!finalCategoryId) return null;
        return buildPath(CATEGORIES, finalCategoryId);
    }, [finalCategoryId]);

    const handleCategorySelect = (level, value) => {
        if (!value) {
            // Bỏ chọn từ level này trở đi
            setSelectedPath((prev) => prev.slice(0, level));
            return;
        }
        setSelectedPath((prev) => {
            const next = prev.slice(0, level);
            next[level] = value;
            return next;
        });
    };

    // -------- Form --------

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    // -------- Images --------

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

    // -------- Submit --------

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            const formPayload = {
                name: form.name,
                description: form.description,
                details: form.details,
                basePrice: form.basePrice,
                totalQuantity: form.totalQuantity,

                categoryId: finalCategoryId,
            };

            const productId = await createProduct(formPayload, images);
            // Sau khi tạo thành công, navigate về trang products hoặc trang detail
            navigate(`/products`);
        } catch (err) {
            setError(err?.response?.data?.message ?? 'Có lỗi xảy ra khi tạo sản phẩm. Vui lòng thử lại.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleReset = () => {
        images.forEach((img) => URL.revokeObjectURL(img.url));
        setImages([]);
        setSelectedPath([]);
        setError(null);
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
                            disabled={isSubmitting}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className={`btn ${styles.btnAccent}`}
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? (
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
                                <label className="form-label small fw-semibold">Description</label>
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
                                <h6 className={`${styles.sectionTitle} mb-0`}>Product Images</h6>
                                <small className="text-muted">{images.length} ảnh đã thêm</small>
                            </div>

                            <div
                                className={styles.dropzone}
                                onDrop={handleDrop}
                                onDragOver={handleDragOver}
                                onClick={() => fileInputRef.current?.click()}
                            >
                                <i className="bi bi-cloud-arrow-up" style={{ fontSize: '1.75rem' }}></i>
                                <div className="fw-semibold mt-2">Kéo & thả ảnh vào đây</div>
                                <small className="text-muted">
                                    hoặc{' '}
                                    <span className={styles.linkAccent}>chọn từ máy tính</span> ·
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

                            {categoryDropdowns.map((dropdown, idx) => (
                                <div key={dropdown.level} className={idx < categoryDropdowns.length - 1 ? 'mb-3' : 'mb-0'}>
                                    <label className="form-label small fw-semibold">
                                        {idx === 0 ? (
                                            <>Category <span className="text-danger">*</span></>
                                        ) : (
                                            `Sub Category (cấp ${idx})`
                                        )}
                                    </label>
                                    <select
                                        className="form-select"
                                        value={dropdown.selectedId}
                                        onChange={(e) => handleCategorySelect(dropdown.level, e.target.value)}
                                        required={idx === 0}
                                    >
                                        <option value="">
                                            {idx === 0 ? '-- Chọn category --' : '-- Chọn sub category --'}
                                        </option>
                                        {dropdown.options.map((opt) => (
                                            <option key={opt.id} value={opt.id}>
                                                {opt.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            ))}

                            {/* Breadcrumb path */}
                            {breadcrumbPath && (
                                <div className={styles.breadcrumbPath} style={{ marginTop: '0.75rem' }}>
                                    <i className="bi bi-diagram-3"></i>
                                    {breadcrumbPath.map((node, idx) => (
                                        <span key={node.id} className="d-flex align-items-center gap-1">
                                            {idx > 0 && (
                                                <i className="bi bi-chevron-right" style={{ fontSize: '0.65rem' }}></i>
                                            )}
                                            <span className={idx === breadcrumbPath.length - 1 ? 'fw-semibold' : ''}>
                                                {node.name}
                                            </span>
                                        </span>
                                    ))}
                                </div>
                            )}

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

export default CreateProduct;
