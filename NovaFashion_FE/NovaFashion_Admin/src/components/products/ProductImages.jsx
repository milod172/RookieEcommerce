import styles from '../../pages/ProductDetails.module.css';

const ProductImages = ({ images, handleSetCover, handleRemoveImage, handleDrop, handleDragOver, fileInputRef, handleFiles, isUploading, isDeleting }) => {
    return (
        <div className={`card border-0 shadow-sm ${styles.panel} mb-3`}>
            {/* Header */}
            <div className="d-flex justify-content-between align-items-center mb-2">
                <h6 className={`${styles.sectionTitle} mb-0`}>Product Images</h6>
                <small className="text-muted">{images.length} ảnh</small>
            </div>

            {/* Image Grid */}
            {images.length > 0 && (
                <div className={styles.imageGrid}>
                    {images.map((img, idx) => (
                        <div key={img.id} className={styles.imageItem}>
                            <img src={img.url} alt={img.name} />

                            {/* Cover badge */}
                            {idx === 0 && (
                                <span className={styles.coverBadge}>Cover</span>
                            )}

                            {/* Overlay loading khi delete */}
                            {isDeleting && (
                                <div className={styles.imageOverlay}>
                                    <span className="spinner-border spinner-border-sm"></span>
                                </div>
                            )}

                            {/* Actions */}
                            <div className={styles.imageActions}>
                                {idx !== 0 && (
                                    <button
                                        type="button"
                                        className={styles.imgActionBtn}
                                        onClick={() => handleSetCover(img.id)}
                                        title="Đặt làm ảnh bìa"
                                        disabled={isUploading || isDeleting}
                                    >
                                        <i className="bi bi-star"></i>
                                    </button>
                                )}

                                <button
                                    type="button"
                                    className={`${styles.imgActionBtn} ${styles.danger}`}
                                    onClick={() => handleRemoveImage(img.id)}
                                    title="Xoá ảnh"
                                    disabled={isUploading || isDeleting}
                                >
                                    <i className="bi bi-trash"></i>
                                </button>

                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Dropzone */}
            <div
                className={`${styles.dropzone} mt-3 ${isUploading ? styles.disabled : ''}`}
                role="button"
                tabIndex={0}
                onDrop={isUploading ? undefined : handleDrop}
                onDragOver={handleDragOver}
                onClick={() => {
                    if (!isUploading) fileInputRef.current?.click();
                }}
                onKeyDown={(e) => {
                    if ((e.key === 'Enter' || e.key === ' ') && !isUploading) {
                        e.preventDefault();
                        fileInputRef.current?.click();
                    }
                }}
            >
                <i className="bi bi-cloud-arrow-up" style={{ fontSize: '1.5rem' }}></i>

                <div className="fw-semibold mt-1">
                    {isUploading ? 'Đang upload...' : 'Kéo & thả thêm ảnh vào đây'}
                </div>

                <small className="text-muted">
                    hoặc <span className={styles.linkAccent}>chọn từ máy tính</span> · PNG, JPG, WEBP
                </small>

                <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    multiple
                    hidden
                    onChange={(e) => {
                        handleFiles(e.target.files);
                        e.target.value = null;
                    }}
                />
            </div>

            {/* Loading state */}
            {(isUploading || isDeleting) && (
                <div className="text-muted small mt-2">
                    {isUploading && 'Đang upload ảnh...'}
                    {isDeleting && 'Đang xoá ảnh...'}
                </div>
            )}
        </div>
    );
};


export default ProductImages;