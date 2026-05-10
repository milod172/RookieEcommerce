const CartService = {

    _getKey() {
        const userId =
            document.getElementById("cart-user-id")?.value ?? "guest";

        return `cart__${userId}`;
    },

    getItems() {

        const raw =
            localStorage.getItem(CartService._getKey());

        if (!raw) return [];

        const cart = JSON.parse(raw);

        if (cart.expiredAt && Date.now() > cart.expiredAt) {

            localStorage.removeItem(CartService._getKey());

            CartService.updateBadge();

            return [];
        }

        return cart.items ?? [];
    },

    _save(items) {

        const cart = {
            items,
            expiredAt: Date.now() + 7 * 24 * 60 * 60 * 1000
        };

        localStorage.setItem(
            CartService._getKey(),
            JSON.stringify(cart)
        );

        CartService.updateBadge();
    },

    add(productVariantId, quantity = 1) {

        const items = CartService.getItems();

        const existing = items.find(
            x => x.product_variant_id === productVariantId
        );

        if (existing) {

            existing.quantity += quantity;

        } else {

            items.push({
                product_variant_id: productVariantId,
                quantity
            });
        }

        CartService._save(items);
    },

    increase(productVariantId) {

        const items = CartService.getItems();

        const item = items.find(
            x => x.product_variant_id === productVariantId
        );

        if (item) {
            item.quantity += 1;
        }

        CartService._save(items);
    },

    decrease(productVariantId) {

        let items = CartService.getItems();

        const item = items.find(
            x => x.product_variant_id === productVariantId
        );

        if (!item) return;

        item.quantity -= 1;

        if (item.quantity <= 0) {

            items = items.filter(
                x => x.product_variant_id !== productVariantId
            );
        }

        CartService._save(items);
    },

    remove(productVariantId) {

        const items = CartService.getItems()
            .filter(
                x => x.product_variant_id !== productVariantId
            );

        CartService._save(items);
    },

    clear() {

        localStorage.removeItem(CartService._getKey());

        CartService.updateBadge();
    },

    totalCount() {

        return CartService.getItems()
            .reduce((sum, x) => sum + x.quantity, 0);
    },

    updateBadge() {

        const badge =
            document.getElementById("cart-badge");

        if (!badge) return;

        const count = CartService.totalCount();

        badge.textContent = count;

        badge.style.display =
            count > 0 ? "inline-flex" : "none";
    },
};

document.addEventListener("DOMContentLoaded", () => {
    CartService.updateBadge();
});