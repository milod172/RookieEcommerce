import { useState, useRef, useMemo, useEffect } from 'react';
import styles from './ProductDetails.module.css';
import { Link } from 'react-router-dom';

// Mock categories — sau này thay bằng API
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

// Mock product — sau này fetch theo id
const MOCK_PRODUCT = {
    id: 'P-00123',
    sku: 'NF-AT-001',
    name: 'Áo thun cotton basic',
    description: 'Áo thun cotton 100% mềm mại, thoáng mát, phù hợp mặc hằng ngày.',
    details:
        'Chất liệu: Cotton 100%\nXuất xứ: Việt Nam\nHướng dẫn giặt: Giặt máy ở nhiệt độ thường, không dùng chất tẩy.',
    basePrice: 199000,
    totalQuantity: 320,
    totalSell: 128,
    categoryId: 'cat-1',
    subCategoryId: 'sub-1-1',
    status: 'active',
    images: [
        { id: 'img-1', url: 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400', name: 'main.jpg' },
        { id: 'img-2', url: 'https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?w=400', name: 'side.jpg' },
        { id: 'img-3', url: 'https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=400', name: 'back.jpg' },
    ],
    variants: [
        { id: 'v-1', skuVariant: 'NF-AT-001-S-WH', stockQuantity: 40, unitPrice: 199000, attrs: 'Size S / Trắng' },
        { id: 'v-2', skuVariant: 'NF-AT-001-M-WH', stockQuantity: 60, unitPrice: 199000, attrs: 'Size M / Trắng' },
        { id: 'v-3', skuVariant: 'NF-AT-001-L-BK', stockQuantity: 50, unitPrice: 209000, attrs: 'Size L / Đen' },
        { id: 'v-4', skuVariant: 'NF-AT-001-XL-BK', stockQuantity: 30, unitPrice: 219000, attrs: 'Size XL / Đen' },
    ],
};

const formatVnd = (n) =>
    new Intl.NumberFormat('vi-VN').format(Number(n) || 0) + '₫';

const ProductDetails = () => {
    const fileInputRef = useRef(null);

    const [form, setForm] = useState(MOCK_PRODUCT);
    const [images, setImages] = useState(MOCK_PRODUCT.images);
    const [variants, setVariants] = useState(MOCK_PRODUCT.variants);
    const [editingVariantId, setEditingVariantId] = useState(null);
    const [variantDraft, setVariantDraft] = useState(null);
    const [isDirty, setIsDirty] = useState(false);

    const subCategories = useMemo(() => {
        const cat = CATEGORIES.find((c) => c.id === form.categoryId);
        return cat?.subCategories ?? [];
    }, [form.categoryId]);

    // cleanup blob URLs khi unmount
    useEffect(() => {
        return () => {
            images.forEach((img) => {
                if (img.url?.startsWith('blob:')) URL.revokeObjectURL(img.url);
            });
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const markDirty = () => setIsDirty(true);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        markDirty();
    };

    const handleCategoryChange = (e) => {
        setForm((prev) => ({
            ...prev,
            categoryId: e.target.value,
            subCategoryId: '',
        }));
        markDirty();
    };

    /* ---------- Images ---------- */
    const handleFiles = (fileList) => {
        const files = Array.from(fileList || []);
        if (!files.length) return;
        const newItems = files.map((file) => ({
            id: `${file.name}-${file.size}-${Date.now()}-${Math.random()}`,
            url: URL.createObjectURL(file),
            name: file.name,
            file,
        }));
        setImages((prev) => [...prev, ...newItems]);
        markDirty();
    };

    const handleRemoveImage = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (target?.url?.startsWith('blob:')) URL.revokeObjectURL(target.url);
            return prev.filter((i) => i.id !== id);
        });
        markDirty();
    };

    const handleSetCover = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (!target) return prev;
            return [target, ...prev.filter((i) => i.id !== id)];
        });
        markDirty();
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (e.dataTransfer?.files?.length) handleFiles(e.dataTransfer.files);
    };
    const handleDragOver = (e) => {
        e.preventDefault();
        e.stopPropagation();
    };

    /* ---------- Variants inline edit ---------- */
    const startEditVariant = (variant) => {
        setEditingVariantId(variant.id);
        setVariantDraft({ ...variant });
    };

    const cancelEditVariant = () => {
        setEditingVariantId(null);
        setVariantDraft(null);
    };

    const saveEditVariant = () => {
        if (!variantDraft) return;
        setVariants((prev) =>
            prev.map((v) => (v.id === variantDraft.id ? { ...variantDraft } : v))
        );
        setEditingVariantId(null);
        setVariantDraft(null);
        markDirty();
    };

    const handleVariantDraftChange = (e) => {
        const { name, value } = e.target;
        setVariantDraft((prev) => ({
            ...prev,
            [name]:
                name === 'stockQuantity' || name === 'unitPrice' ? Number(value) : value,
        }));
    };

    /* ---------- Save / Reset ---------- */
    const handleSave = (e) => {
        e.preventDefault();
        const payload = { ...form, images, variants };
        console.log('Save product:', payload);
        setIsDirty(false);
    };

    const handleDiscard = () => {
        setForm(MOCK_PRODUCT);
        setImages(MOCK_PRODUCT.images);
        setVariants(MOCK_PRODUCT.variants);
        setEditingVariantId(null);
        setVariantDraft(null);
        setIsDirty(false);
    };

    return (
        <div className="container-fluid">
            <form onSubmit={handleSave}>
                {/* Top header */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <div className="d-flex align-items-center gap-2 text-muted small mb-1">
                            <i className="bi bi-box-seam"></i>
                            <Link to="/products" className={styles.backDecorationNone}><span>Products</span></Link>
                            <i className="bi bi-chevron-right" style={{ fontSize: '0.7rem' }}></i>
                            <span className="text-dark">{form.name || 'Product details'}</span>
                        </div>
                        <div className="d-flex align-items-center gap-2">
                            <h5 className="fw-bold mb-0">{form.name || 'Product details'}</h5>
                            <span
                                className={`${styles.statusPill} ${form.status === 'active' ? styles.active : styles.inactive
                                    }`}
                            >
                                {form.status === 'active' ? 'Active' : 'Inactive'}
                            </span>
                        </div>
                        <small className="text-muted">
                            ID: <span className="fw-semibold">{form.id}</span> · SKU:{' '}
                            <span className="fw-semibold">{form.sku}</span>
                        </small>
                    </div>

                    <div className="d-flex gap-2 align-items-center">
                        {isDirty && (
                            <span className={styles.dirtyDot}>
                                <i className="bi bi-circle-fill"></i> Có thay đổi chưa lưu
                            </span>
                        )}
                        <button
                            type="button"
                            className="btn btn-light border"
                            onClick={handleDiscard}
                            disabled={!isDirty}
                        >
                            Discard
                        </button>
                        <button
                            type="submit"
                            className={`btn ${styles.btnAccent}`}
                            disabled={!isDirty}
                        >
                            <i className="bi bi-check2"></i> Save Changes
                        </button>
                    </div>
                </div>

                <div className="row g-3">
                    {/* LEFT — Main info */}
                    <div className="col-lg-8">
                        {/* General info — inline edit */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>General Information</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">
                                    Product Name
                                </label>
                                <input
                                    type="text"
                                    name="name"
                                    value={form.name}
                                    onChange={handleChange}
                                    className={`form-control ${styles.inlineInput}`}
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
                                    className={`form-control ${styles.inlineInput}`}
                                    rows={3}
                                />
                            </div>

                            <div className="mb-0">
                                <label className="form-label small fw-semibold">Details</label>
                                <textarea
                                    name="details"
                                    value={form.details}
                                    onChange={handleChange}
                                    className={`form-control ${styles.inlineInput}`}
                                    rows={5}
                                />
                            </div>
                        </div>

                        {/* Images */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <div className="d-flex justify-content-between align-items-center mb-2">
                                <h6 className={`${styles.sectionTitle} mb-0`}>
                                    Product Images
                                </h6>
                                <small className="text-muted">{images.length} ảnh</small>
                            </div>

                            {images.length > 0 && (
                                <div className={styles.imageGrid}>
                                    {images.map((img, idx) => (
                                        <div key={img.id} className={styles.imageItem}>
                                            <img src={img.url} alt={img.name} />
                                            {idx === 0 && (
                                                <span className={styles.coverBadge}>Cover</span>
                                            )}
                                            <div className={styles.imageActions}>
                                                {idx !== 0 && (
                                                    <button
                                                        type="button"
                                                        className={styles.imgActionBtn}
                                                        onClick={() => handleSetCover(img.id)}
                                                        title="Đặt làm ảnh bìa"
                                                    >
                                                        <i className="bi bi-star"></i>
                                                    </button>
                                                )}
                                                <button
                                                    type="button"
                                                    className={`${styles.imgActionBtn} ${styles.danger}`}
                                                    onClick={() => handleRemoveImage(img.id)}
                                                    title="Xoá ảnh"
                                                >
                                                    <i className="bi bi-trash"></i>
                                                </button>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}

                            <div
                                className={`${styles.dropzone} mt-3`}
                                onDrop={handleDrop}
                                onDragOver={handleDragOver}
                                onClick={() => fileInputRef.current?.click()}
                            >
                                <i
                                    className="bi bi-cloud-arrow-up"
                                    style={{ fontSize: '1.5rem' }}
                                ></i>
                                <div className="fw-semibold mt-1">
                                    Kéo & thả thêm ảnh vào đây
                                </div>
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
                        </div>

                        {/* Variants */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <div className="d-flex justify-content-between align-items-center mb-2">
                                <h6 className={`${styles.sectionTitle} mb-0`}>
                                    Variants ({variants.length})
                                </h6>
                                <small className="text-muted">
                                    Click <i className="bi bi-pencil-square"></i> để chỉnh sửa
                                    trực tiếp
                                </small>
                            </div>

                            <div className="table-responsive">
                                <table className={`table align-middle mb-0 ${styles.variantTable}`}>
                                    <thead>
                                        <tr>
                                            <th>SKU Variant</th>
                                            <th>Thuộc tính</th>
                                            <th className="text-end">Stock Quantity</th>
                                            <th className="text-end">Unit Price</th>
                                            <th className="text-center" style={{ width: 120 }}>
                                                Action
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {variants.map((v) => {
                                            const isEditing = editingVariantId === v.id;
                                            return (
                                                <tr key={v.id} className={isEditing ? styles.editingRow : ''}>
                                                    <td>
                                                        {isEditing ? (
                                                            <input
                                                                type="text"
                                                                name="skuVariant"
                                                                value={variantDraft.skuVariant}
                                                                onChange={handleVariantDraftChange}
                                                                className="form-control form-control-sm"
                                                            />
                                                        ) : (
                                                            <span className="fw-semibold">{v.skuVariant}</span>
                                                        )}
                                                    </td>
                                                    <td>
                                                        <small className="text-muted">{v.attrs}</small>
                                                    </td>
                                                    <td className="text-end">
                                                        {isEditing ? (
                                                            <input
                                                                type="number"
                                                                name="stockQuantity"
                                                                value={variantDraft.stockQuantity}
                                                                onChange={handleVariantDraftChange}
                                                                className="form-control form-control-sm text-end"
                                                                min="0"
                                                            />
                                                        ) : (
                                                            v.stockQuantity
                                                        )}
                                                    </td>
                                                    <td className="text-end">
                                                        {isEditing ? (
                                                            <input
                                                                type="number"
                                                                name="unitPrice"
                                                                value={variantDraft.unitPrice}
                                                                onChange={handleVariantDraftChange}
                                                                className="form-control form-control-sm text-end"
                                                                min="0"
                                                            />
                                                        ) : (
                                                            formatVnd(v.unitPrice)
                                                        )}
                                                    </td>
                                                    <td className="text-center">
                                                        {isEditing ? (
                                                            <div className="d-flex justify-content-center gap-1">
                                                                <button
                                                                    type="button"
                                                                    className={`btn btn-sm ${styles.btnAccent}`}
                                                                    onClick={saveEditVariant}
                                                                    title="Lưu"
                                                                >
                                                                    <i className="bi bi-check-lg"></i>
                                                                </button>
                                                                <button
                                                                    type="button"
                                                                    className="btn btn-sm btn-light border"
                                                                    onClick={cancelEditVariant}
                                                                    title="Huỷ"
                                                                >
                                                                    <i className="bi bi-x-lg"></i>
                                                                </button>
                                                            </div>
                                                        ) : (
                                                            <button
                                                                type="button"
                                                                className={`btn btn-sm btn-light border ${styles.editBtn}`}
                                                                onClick={() => startEditVariant(v)}
                                                                disabled={
                                                                    editingVariantId !== null &&
                                                                    editingVariantId !== v.id
                                                                }
                                                            >
                                                                <i className="bi bi-pencil-square"></i> Edit
                                                            </button>
                                                        )}
                                                    </td>
                                                </tr>
                                            );
                                        })}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    {/* RIGHT — Side panels */}
                    <div className="col-lg-4">
                        {/* Stats */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Performance</h6>
                            <div className="row g-2">
                                <div className="col-6">
                                    <div className={styles.statBox}>
                                        <small className="text-muted">Total Quantity</small>
                                        <div className="fw-bold fs-5">{form.totalQuantity}</div>
                                    </div>
                                </div>
                                <div className="col-6">
                                    <div className={styles.statBox}>
                                        <small className="text-muted">Total Sell</small>
                                        <div className="fw-bold fs-5">{form.totalSell}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Pricing & inventory */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Pricing & Inventory</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">Base Price</label>
                                <div className="input-group">
                                    <span className="input-group-text">₫</span>
                                    <input
                                        type="number"
                                        name="basePrice"
                                        value={form.basePrice}
                                        onChange={handleChange}
                                        className={`form-control ${styles.inlineInput}`}
                                        min="0"
                                    />
                                </div>
                            </div>

                            <div className="mb-0">
                                <label className="form-label small fw-semibold">
                                    Total Quantity
                                </label>
                                <input
                                    type="number"
                                    name="totalQuantity"
                                    value={form.totalQuantity}
                                    onChange={handleChange}
                                    className={`form-control ${styles.inlineInput}`}
                                    min="0"
                                />
                                <small className="text-muted">
                                    Tổng tồn kho = tổng stock của các variants.
                                </small>
                            </div>
                        </div>

                        {/* Cascading category */}
                        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
                            <h6 className={styles.sectionTitle}>Categorization</h6>

                            <div className="mb-3">
                                <label className="form-label small fw-semibold">Category</label>
                                <select
                                    name="categoryId"
                                    value={form.categoryId}
                                    onChange={handleCategoryChange}
                                    className={`form-select ${styles.inlineInput}`}
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
                                    className={`form-select ${styles.inlineInput}`}
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
                                                <i
                                                    className="bi bi-chevron-right"
                                                    style={{ fontSize: '0.65rem' }}
                                                ></i>
                                                <span className="fw-semibold">
                                                    {
                                                        subCategories.find(
                                                            (s) => s.id === form.subCategoryId
                                                        )?.name
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
                                className={`form-select ${styles.inlineInput}`}
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

export default ProductDetails;
