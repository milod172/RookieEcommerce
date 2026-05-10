using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace NovaFashion.API.Features.VnPay
{
    public class VnPayRequest
    {
        public Guid OrderId { get; set; }
    }

    public class CreateUrlPayment(IVnpayClient vnpayClient, AppDbContext db) : Endpoint<VnPayRequest, string>
    {
        public override void Configure()
        {
            Post("url-payment");
            Group<VnPayGroup>();
            Roles(Role.Customer.ToString());
        }

        public override async Task HandleAsync(VnPayRequest req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");

            if (currentUserId == null)
            {
                ThrowError("Vui lòng đăng nhập", statusCode: 401); 
                return;
            }

            var order = await db.Orders.FirstOrDefaultAsync(x => x.CustomerId == currentUserId, ct);

            if (order == null)
            {
                ThrowError("Không tìm thấy đơn hàng", statusCode: 404);
                return;
            }

            if(order.OrderStatus != OrderStatus.Pending)
            {
                ThrowError("Đơn hàng đã được thanh toán", statusCode: 400);
                return;
            }


            var request = new VnpayPaymentRequest
            {
                Money = (double)order.TotalAmount,
                Description = $"Thanh toán đơn hàng {order.Id.ToString()[..6]}",
                BankCode = BankCode.ANY
            };

            var paymentUrlInfo = vnpayClient.CreatePaymentUrl(request);
            var paymentUrl = paymentUrlInfo.Url;

            await Send.OkAsync(paymentUrl, ct);
        }
    }
}
