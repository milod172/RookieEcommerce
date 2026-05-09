const CART_KEY = "cart";

const CartService = {

    // Đọc cart từ LocalStorage
    getItems() {
        const raw = localStorage.getItem(CART_KEY);
        return raw ? JSON.parse(raw) : [];
    },

    // Ghi cart xuống LocalStorage
    _save(items) {
        localStorage.setItem(CART_KEY, JSON.stringify(items));
        CartService.updateBadge();
    },

    // Thêm hoặc tăng quantity
    add(productVariantId, quantity = 1) {
        const items = CartService.getItems();
        const existing = items.find(x => x.productVariantId === productVariantId);
        if (existing) {
            existing.quantity += quantity;
        } else {
            items.push({ productVariantId, quantity });
        }
        CartService._save(items);
    },

    // Tăng 1
    increase(productVariantId) {
        const items = CartService.getItems();
        const item = items.find(x => x.productVariantId === productVariantId);
        if (item) item.quantity += 1;
        CartService._save(items);
    },

    // Giảm 1, nếu về 0 thì xoá
    decrease(productVariantId) {
        let items = CartService.getItems();
        const item = items.find(x => x.productVariantId === productVariantId);
        if (!item) return;
        item.quantity -= 1;
        if (item.quantity <= 0) {
            items = items.filter(x => x.productVariantId !== productVariantId);
        }
        CartService._save(items);
    },

    // Xoá 1 item
    remove(productVariantId) {
        const items = CartService.getItems()
            .filter(x => x.productVariantId !== productVariantId);
        CartService._save(items);
    },

    // Xoá toàn bộ cart (sau checkout)
    clear() {
        localStorage.removeItem(CART_KEY);
        CartService.updateBadge();
    },

    // Tổng số lượng item trong giỏ
    totalCount() {
        return CartService.getItems().reduce((sum, x) => sum + x.quantity, 0);
    },

    // Cập nhật badge số lượng trên header
    updateBadge() {
        const badge = document.getElementById("cart-badge");
        if (!badge) return;
        const count = CartService.totalCount();
        badge.textContent = count;
        badge.style.display = count > 0 ? "inline-flex" : "none";
    },
};

document.addEventListener("DOMContentLoaded", () => {
    CartService.updateBadge();
});