export const formatVnd = (n) =>
    new Intl.NumberFormat('vi-VN').format(Number(n) || 0) + '₫';