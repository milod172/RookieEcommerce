import { useState, useRef, useEffect } from 'react';

export const useProductImages = () => {
    const [images, setImages] = useState([]);
    const fileInputRef = useRef(null);

    // Cleanup blob URLs khi component unmount
    useEffect(() => {
        return () => {
            images.forEach((img) => {
                if (img.url?.startsWith('blob:')) URL.revokeObjectURL(img.url);
            });
        };
    }, [images]);

    const handleFiles = (fileList) => {
        const files = Array.from(fileList || []);
        if (!files.length) return;

        const newItems = files.map((file) => ({
            id: `${file.name}-${file.size}-${Date.now()}-${Math.random()}`,
            url: URL.createObjectURL(file),
            name: file.name,
            file, // giữ file thật để gửi lên server sau
        }));

        setImages((prev) => [...prev, ...newItems]);
    };

    const handleRemoveImage = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (target?.url?.startsWith('blob:')) URL.revokeObjectURL(target.url);
            return prev.filter((i) => i.id !== id);
        });
    };

    const handleSetCover = (id) => {
        setImages((prev) => {
            const target = prev.find((i) => i.id === id);
            if (!target) return prev;
            return [target, ...prev.filter((i) => i.id !== id)];
        });
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

    const clearImages = () => {
        images.forEach((img) => URL.revokeObjectURL(img.url));
        setImages([]);
    };

    return {
        images,
        setImages,
        handleFiles,
        handleRemoveImage,
        handleSetCover,
        handleDrop,
        handleDragOver,
        fileInputRef,
        clearImages,
    };
};