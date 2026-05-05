import { useMemo } from 'react';
import { categoryApi } from '../../features/categories/categoryApi';
import useSWR from 'swr';

// Flat hóa toàn bộ tree đệ quy vào nodeMap
const flattenTree = (nodes, map = {}) => {
    nodes?.forEach(node => {
        map[node.id] = node;
        if (node.sub_categories?.length > 0) {
            flattenTree(node.sub_categories, map);
        }
    });
    return map;
};

export const useCategoryTree = (selectedPath, setSelectedPath) => {

    // Dùng getAll — data đã có sub_categories nested sẵn
    const { data, isLoading, error } = useSWR(
        'categories/tree-picker',
        () => categoryApi.getAll({ PageNumber: 1, PageSize: 100 }),
        { revalidateOnFocus: false }
    );

    // Root categories từ paginated response
    const rootCategories = data?.items ?? [];

    // nodeMap flat toàn bộ tree để lookup nhanh theo id
    const nodeMap = useMemo(() => flattenTree(rootCategories), [rootCategories]);

    // Build dropdowns theo selectedPath — hỗ trợ unlimited depth
    const categoryDropdowns = useMemo(() => {
        if (!rootCategories.length) return [];

        const dropdowns = [{
            level: 0,
            options: rootCategories,
            selectedId: selectedPath[0] ?? '',
        }];

        for (let i = 0; i < selectedPath.length; i++) {
            const selectedNode = nodeMap[selectedPath[i]];
            if (!selectedNode?.sub_categories?.length) break;

            dropdowns.push({
                level: i + 1,
                options: selectedNode.sub_categories,
                selectedId: selectedPath[i + 1] ?? '',
            });
        }

        return dropdowns;
    }, [rootCategories, selectedPath, nodeMap]);

    const finalCategoryId = selectedPath.length > 0
        ? selectedPath[selectedPath.length - 1]
        : null;

    const breadcrumbPath = useMemo(() => {
        if (!selectedPath.length) return [];
        return selectedPath.map(id => nodeMap[id]).filter(Boolean);
    }, [selectedPath, nodeMap]);

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
        handleCategorySelect,
        isLoading,
        isError: !!error,
    };
};