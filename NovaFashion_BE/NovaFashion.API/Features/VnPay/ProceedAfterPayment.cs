using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using VNPAY;

namespace NovaFashion.API.Features.VnPay
{
    public class VnPayCallbackRequest { 
        public string vnp_Amount { get; set; } = string.Empty;
        public string vnp_BankCode { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TransactionStatus { get; set; } = string.Empty;
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string np_SecureHash { get; set; } = string.Empty;
    }

    public class ProceedAfterPayment(IVnpayClient vnpayClient, AppDbContext db) : Endpoint<VnPayCallbackRequest, string>
    {
        public override void Configure()
        {
            Get("callback");
            Group<VnPayGroup>();
            AllowAnonymous();  
        }

        public override async Task HandleAsync(VnPayCallbackRequest req,CancellationToken ct)
        {
            var amount = req.vnp_Amount;
            var orderId = Guid.Parse(req.vnp_OrderInfo!);
            var status = req.vnp_TransactionStatus;

            if (status != "00")
            {
                await Send.RedirectAsync($"/payment-failed?orderId={orderId}", allowRemoteRedirects: false);
                return;
            }

            var order = await db.Orders
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId, ct);

            if (order == null)
            {
                await Send.RedirectAsync("/payment-failed", allowRemoteRedirects: false);
                return;
            }

            foreach (var item in order.OrderItems)
            {
                var variant = item.ProductVariant;
                var product = variant.Product;

                variant.StockQuantity -= item.Quantity;
                product.TotalQuantity -= item.Quantity;
                product.TotalSell += item.Quantity;

            }

            order.PaymentStatus = PaymentStatus.Succeed;
            order.OrderStatus = OrderStatus.Completed;
            order.PaymentDate = DateTime.UtcNow;
            order.TransactionId = req.vnp_TxnRef;

            await db.SaveChangesAsync(ct);

            await Send.RedirectAsync($"http://localhost:5266/payment-success/{orderId}",allowRemoteRedirects: true);
        }
    }
}
