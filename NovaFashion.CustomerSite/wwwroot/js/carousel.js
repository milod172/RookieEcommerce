export const carousel_contents = [
    {
        id: 1,
        title: "Bộ Sưu Tập Vest Nam Mới",
        content: "Nâng tầm tủ quần áo của bạn với bộ sưu tập vest nam mới, mang đến những thiết kế vượt thời gian với phong cách hiện đại. Được chế tác từ các loại vải cao cấp, bộ sưu tập này cung cấp nhiều màu sắc và kiểu dáng phù hợp với mọi dịp. Tạo dấu ấn với phong cách thanh lịch và tinh tế.",
        url: "https://res.cloudinary.com/novafashion/image/upload/v1776763928/banner_01_knnrrc.jpg"
    },
    {
        id: 2,
        title: "Bộ Sưu Tập Xuân 2026 Mới",
        content: "Giới thiệu bộ sưu tập thời trang xuân 2026 mới, hoàn hảo cho phong cách casual nhẹ nhàng. Từ những chiếc váy bay bổng đến áo hoa, dễ dàng phối hợp. Vải nhẹ và màu sắc tươi sáng để đánh thức mùa xuân trong bạn.",
        url: "https://res.cloudinary.com/novafashion/image/upload/v1776763928/banner_02_ktazmj.jpg"
    },
    {
        id: 3,
        title: "Bộ Sưu Tập Hè 2026 Mới",
        content: "Giới thiệu bộ sưu tập thời trang hè 2026 mới, với màu sắc tươi sáng, họa tiết vui nhộn và những món đồ thoải mái nhưng vẫn phong cách, hoàn hảo cho mọi dịp. Từ những chiếc váy blazer oversized đến các món đồ denim đơn sắc, bộ sưu tập này có điều gì đó cho mọi người.",
        url: "https://res.cloudinary.com/novafashion/image/upload/v1776763928/banner_03_jn20ni.jpg"
    },
]

export function renderCarousel(data) {
    const carouselItems = document.querySelector(".carousel-inner");

    carouselItems.innerHTML = "";

    data.forEach((item, index) => {
        carouselItems.innerHTML += `
        <div class="carousel-item ${index === 0 ? "active" : ""}" data-bs-interval="5000">
            <div class="row">
                <div class="col-md-4 carousel-content">
                    <h1 class="mb-4">${item.title}</h1>
                    <p class="mb-4">${item.content}</p>
                    <button class="btn btn-dark">SHOP NOW</button>
                </div>

                <div class="col-md-8 px-0">
                    <img src="${item.url}" class="d-block w-100 h-100 object-fit-cover" alt="">
                </div>
            </div>
        </div>`;
    });
}