import { useState, useRef, useEffect } from 'react';
import { productApi } from '../../features/products/productApi';

export const useProductImages = (productId = null, mutateProduct) => {
    const [images, setImages] = useState([]);
    const [isUploading, setIsUploading] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);
    const [uploadErrors, setUploadErrors] = useState([]);
    const fileInputRef = useRef(null);

    // Cleanup blob
    useEffect(() => {
        return () => {
            images.forEach((img) => {
                if (img.url?.startsWith('blob:')) {
                    URL.revokeObjectURL(img.url);
                }
            });
        };
    }, [images]);


    // UPLOAD
    const handleFiles = async (fileList) => {
        const files = Array.from(fileList || []);
        if (!files.length) return;


        if (!productId) {
            const newItems = files.map((file) => ({
                id: `${file.name}-${Date.now()}`,
                url: URL.createObjectURL(file),
                name: file.name,
                file,
                isNew: true,
            }));

            setImages((prev) => [...prev, ...newItems]);
            return;
        }

        setIsUploading(true);
        setUploadErrors([]);

        try {
            const formData = new FormData();
            files.forEach((file) => formData.append('files', file));

            const uploaded = await productApi.uploadImages(productId, formData);

            const newItems = uploaded.flat().map((img) => ({
                id: img.id,
                url: img.image_url,
                name: img.alt_text,
                isPrimary: img.is_primary,
                sortOrder: img.sort_order,
                isNew: false,
            }));

            setImages((prev) => [...prev, ...newItems]);
            mutateProduct();

        } catch (err) {
            console.error("Upload failed", err);
            const data = err?.response?.data;

            if (data?.errors) {
                // Parse "files[0]", "files[1]" → flat list thông báo lỗi
                const messages = Object.values(data.errors).flat();
                setUploadErrors(messages);
            } else {
                setUploadErrors(["Upload thất bại, vui lòng thử lại."]);
            }

        } finally {
            setIsUploading(false);
        }
    };


    const handleRemoveImage = async (id) => {
        const target = images.find((i) => i.id === id);
        if (!target) return;


        if (!productId || target.isNew) {
            if (target.url?.startsWith('blob:')) {
                URL.revokeObjectURL(target.url);
            }
            setImages((prev) => prev.filter((i) => i.id !== id));
            return;
        }

        setIsDeleting(true);

        try {
            await productApi.deleteImage(productId, target.id);


            setImages((prev) =>
                prev
                    .filter((i) => i.id !== id)
                    .map((img, index) => ({
                        ...img,
                        sortOrder: index,
                        isPrimary: index === 0,
                    }))
            );

            mutateProduct();

        } catch (err) {
            console.error("Delete failed", err);
        } finally {
            setIsDeleting(false);
        }
    };


    // Set Order
    const handleSetCover = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (!target) return prev;

            return [
                { ...target, isPrimary: true },
                ...prev
                    .filter((i) => i.id !== id)
                    .map((img) => ({ ...img, isPrimary: false })),
            ];
        });
    };

    const handleDrop = (e) => {
        e.preventDefault();
        if (e.dataTransfer?.files?.length) {
            handleFiles(e.dataTransfer.files);
        }
    };

    const handleDragOver = (e) => e.preventDefault();

    const clearImages = () => {
        images.forEach((img) => {
            if (img.url?.startsWith('blob:')) {
                URL.revokeObjectURL(img.url);
            }
        });
        setImages([]);
    };

    return {
        images,
        setImages,
        isUploading,
        isDeleting,
        handleFiles,
        handleRemoveImage,
        handleSetCover,
        handleDrop,
        handleDragOver,
        fileInputRef,
        clearImages,
        uploadErrors,
        setUploadErrors
    };
};