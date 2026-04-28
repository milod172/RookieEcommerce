import { useMemo } from "react";

export const usePagination = ({ page, totalCount, pageSize }) => {
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

    const pageNumbers = useMemo(() => {
        const pages = [];
        const windowSize = 1;
        for (let i = 1; i <= totalPages; i++) {
            if (
                i === 1 ||
                i === totalPages ||
                (i >= page - windowSize && i <= page + windowSize)
            ) {
                pages.push(i);
            } else if (pages[pages.length - 1] !== "...") {
                pages.push("...");
            }
        }
        return pages;
    }, [page, totalPages]);

    const startIdx = (page - 1) * pageSize + 1;
    const endIdx = Math.min(page * pageSize, totalCount);

    return { totalPages, pageNumbers, startIdx, endIdx };
};