import { useMemo, useState } from 'react';
import { categoryApi } from "../../features/categories/categoryApi";
export const useCategoryTree = (rootCategories, selectedPath, setSelectedPath) => {

    // Cache tất cả sub đã fetch: { [parentId]: { options, isLoading } }
    const [subCache, setSubCache] = useState({});

    // Fetch sub theo parentId, tự update vào cache
    const fetchSub = async (parentId) => {
        if (!parentId || subCache[parentId]) return; // đã có cache thì thôi

        setSubCache(prev => ({
            ...prev,
            [parentId]: { options: [], isLoading: true }
        }));

        try {
            const data = await categoryApi.getSubCategories(parentId);
            setSubCache(prev => ({
                ...prev,
                [parentId]: { options: data, isLoading: false }
            }));
        } catch {
            setSubCache(prev => ({
                ...prev,
                [parentId]: { options: [], isLoading: false }
            }));
        }
    };

    // Build nodeMap từ rootCategories + tất cả sub đã cache
    const nodeMap = useMemo(() => {
        const map = {};

        rootCategories?.forEach(node => { map[node.id] = node; });

        Object.values(subCache).forEach(({ options }) => {
            options?.forEach(node => { map[node.id] = node; });
        });

        return map;
    }, [rootCategories, subCache]);

    // Build dropdowns động theo selectedPath
    const categoryDropdowns = useMemo(() => {
        if (!rootCategories?.length) return [];

        const dropdowns = [{
            level: 0,
            options: rootCategories,
            selectedId: selectedPath[0] ?? '',
            isLoading: false,
        }];

        for (let i = 0; i < selectedPath.length; i++) {
            const selectedNode = nodeMap[selectedPath[i]];

            if (!selectedNode?.has_children) break;

            const cached = subCache[selectedPath[i]];

            dropdowns.push({
                level: i + 1,
                options: cached?.options ?? [],
                selectedId: selectedPath[i + 1] ?? '',
                isLoading: cached?.isLoading ?? false,
            });

            if (!cached || cached.isLoading) break;
        }

        return dropdowns;
    }, [rootCategories, selectedPath, nodeMap, subCache]);

    const finalCategoryId = selectedPath.length > 0
        ? selectedPath[selectedPath.length - 1]
        : null;

    const breadcrumbPath = useMemo(() => {
        if (!finalCategoryId) return null;
        return selectedPath.map(id => nodeMap[id]).filter(Boolean);
    }, [selectedPath, nodeMap]);

    // Khi user chọn 1 option → fetch sub của nó nếu has_children
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

        const selectedNode = nodeMap[value];
        if (selectedNode?.has_children) {
            fetchSub(value); // trigger fetch, tự cache
        }
    };

    const buildPathFromId = (ancestorIds = []) => {
        return ancestorIds.filter(Boolean);
    };

    return {
        categoryDropdowns,
        finalCategoryId,
        breadcrumbPath,
        handleCategorySelect,
        buildPathFromId,
    };
};