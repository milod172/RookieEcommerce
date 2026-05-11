using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.OrdersTable
{
    public class GetPaymentSuccessRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid OrderId { get; set; }
    }


    public class GetPaymentSuccessMapper : Mapper<GetPaymentSuccessRequest, GetPaymentSuccessDto, OrderItem>
    {
        public override GetPaymentSuccessDto FromEntity(OrderItem o)
        {
            return new GetPaymentSuccessDto
            {
                OrderId = o.OrderId,
                ProductName = $"{o.ProductVariant?.Product.ProductName} - {o.ProductVariant?.Size}",
                CreateTime = o.Order?.CreatedTime.ToString("dd/MM/yyyy"),
                PaymentMethod = o.Order?.PaymentMethod.ToString(),
                TotalAmount = o.Order.TotalAmount,
            };
        }
    }
    public class GetPaymentSuccess(AppDbContext db) : Endpoint<GetPaymentSuccessRequest, GetPaymentSuccessDto, GetPaymentSuccessMapper>
    {
        public override void Configure()
        {
            Get("payment-success/{id}");
            AllowAnonymous();
            Group<OrderGroup>();
        }

        public override async Task HandleAsync(GetPaymentSuccessRequest req, CancellationToken ct)
        {
            var orderItem = await db.OrderItems
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.OrderId == req.OrderId, ct);

            if (orderItem == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            var response = Map.FromEntity(orderItem);
            await Send.OkAsync(response, ct);
        }
    }
}
