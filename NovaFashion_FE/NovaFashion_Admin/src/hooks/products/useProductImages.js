import { useState } from 'react';

export const useProductImages = () => {
    const [images, setImages] = useState([]);

    const handleFiles = (fileList) => {
        const files = Array.from(fileList || []);

        const newItems = files.map(file => ({
            id: `${file.name}-${file.size}-${Date.now()}-${Math.random()}`,
            url: URL.createObjectURL(file),
            name: file.name,
            file
        }));

        setImages(prev => [...prev, ...newItems]);
    };

    const handleRemoveImage = (id) => {
        setImages(prev => {
            const target = prev.find(i => i.id === id);
            if (target) URL.revokeObjectURL(target.url);
            return prev.filter(i => i.id !== id);
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

    const clearImages = () => {
        images.forEach(img => URL.revokeObjectURL(img.url));
        setImages([]);
    };

    return {
        images,
        handleFiles,
        handleRemoveImage,
        handleDrop,
        handleDragOver,
        clearImages
    };
};