import { useMemo } from 'react';

export const useCategoryTree = (CATEGORIES, selectedPath, setSelectedPath) => {

    const findNode = (nodes, id) => {
        for (const node of nodes) {
            if (node.id === id) return node;
            if (node.sub_categories?.length) {
                const found = findNode(node.sub_categories, id);
                if (found) return found;
            }
        }
        return null;
    };

    const buildPath = (nodes, targetId, path = []) => {
        for (const node of nodes) {
            const current = [...path, node];
            if (node.id === targetId) return current;

            if (node.sub_categories?.length) {
                const found = buildPath(node.sub_categories, targetId, current);
                if (found) return found;
            }
        }
        return null;
    };

    const categoryDropdowns = useMemo(() => {
        if (!CATEGORIES.length) return [];

        const dropdowns = [{
            level: 0,
            options: CATEGORIES,
            selectedId: selectedPath[0] ?? ''
        }];

        for (let i = 0; i < selectedPath.length; i++) {
            const node = findNode(CATEGORIES, selectedPath[i]);
            if (node?.sub_categories?.length > 0) {
                dropdowns.push({
                    level: i + 1,
                    options: node.sub_categories,
                    selectedId: selectedPath[i + 1] ?? ''
                });
            } else {
                break;
            }
        }

        return dropdowns;
    }, [CATEGORIES, selectedPath]);

    const finalCategoryId =
        selectedPath.length > 0
            ? selectedPath[selectedPath.length - 1]
            : null;

    const breadcrumbPath = useMemo(() => {
        if (!finalCategoryId || !CATEGORIES.length) return null;
        return buildPath(CATEGORIES, finalCategoryId);
    }, [CATEGORIES, finalCategoryId]);

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