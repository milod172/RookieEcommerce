import { useMemo } from 'react';

export const useCategoryTree = (CATEGORIES, selectedPath, setSelectedPath) => {

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

    const categoryDropdowns = useMemo(() => {
        const dropdowns = [
            {
                level: 0,
                options: CATEGORIES,
                selectedId: selectedPath[0] ?? ''
            }
        ];

        for (let i = 0; i < selectedPath.length; i++) {
            const node = findNode(CATEGORIES, selectedPath[i]);

            if (node?.subCategories?.length > 0) {
                dropdowns.push({
                    level: i + 1,
                    options: node.subCategories,
                    selectedId: selectedPath[i + 1] ?? ''
                });
            } else {
                break;
            }
        }

        return dropdowns;
    }, [selectedPath]);

    const finalCategoryId =
        selectedPath.length > 0
            ? selectedPath[selectedPath.length - 1]
            : null;

    const breadcrumbPath = useMemo(() => {
        if (!finalCategoryId) return null;
        return buildPath(CATEGORIES, finalCategoryId);
    }, [finalCategoryId]);

    const handleCategorySelect = (level, value) => {
        if (!value) {
            setSelectedPath(prev => prev.slice(0, level));
            return;
        }

        setSelectedPath(prev => {
            const next = prev.slice(0, level);
            next[level] = value;
            return next;
        });
    };

    return {
        categoryDropdowns,
        finalCategoryId,
        breadcrumbPath,
        handleCategorySelect
    };
};