using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CartDtos;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.OrdersTable
{
    public class GetOrderDetailsRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
    }

    public class GetOrderDetailsMapper : Mapper<GetOrderDetailsRequest, OrderDetailsDto, Orders>
    {
        public override OrderDetailsDto FromEntity(Orders e)
        {
            return new OrderDetailsDto
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                PhoneNumber = e.PhoneNumber,
                Address = e.ShippingAddress,
                CreateTime = e.CreatedTime.ToString("dd/MM/yyyy HH:mm"),
                PaymentMethod = e.PaymentMethod.ToString(),
                OrderStatus = e.OrderStatus.ToString(),
                TotalAmount = e.TotalAmount,
                OrderItems = e.OrderItems.Select(oi => new CartItemDto
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductId = oi.ProductVariant!.ProductId,
                    ProductName = oi.ProductVariant!.Product!.ProductName,
                    ProductDescription = oi.ProductVariant!.Product!.Description,
                    ImageUrl = oi.ProductVariant!.Product!.ProductImages
                        .FirstOrDefault(x => x.IsPrimary)?.ImageUrl ?? string.Empty,
                    Size = oi.ProductVariant!.Size.ToString(),
                    Sku = oi.ProductVariant!.VariantSku,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.UnitPrice * oi.Quantity,
                }).ToList()
            };
        }
    }

    public class GetOrderDetails(AppDbContext db) : Endpoint<GetOrderDetailsRequest, OrderDetailsDto, GetOrderDetailsMapper>
    {
        public override void Configure()
        {
            Get("{id}");
            Roles(Role.Customer.ToString(), Role.Admin.ToString());
            Group<OrderGroup>();
        }

        public override async Task HandleAsync(GetOrderDetailsRequest req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");
            if (currentUserId == null)
            {
                ThrowError("Vui lòng đăng nhập lại", statusCode: 401);
                return;
            }

            var isAdmin = User.IsInRole(Role.Admin.ToString());
  
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
            if (order == null)
            {
                ThrowError("Không tìm thấy Order", statusCode: 404);
                return;
            }

            var result = await db.Orders
               .Where(x => x.Id == order.Id)
               .Include(x => x.OrderItems)
                   .ThenInclude(x => x.ProductVariant)
                       .ThenInclude(x => x.Product)
                           .ThenInclude(x => x.ProductImages)
               .FirstAsync(ct);

            if (isAdmin)
            {
                await Send.OkAsync(Map.FromEntity(result), ct);
                return;
            }

            if(order.CustomerId != currentUserId)
            {
                ThrowError("Không tìm thấy Order", statusCode: 404);
                return;
            }

            await Send.OkAsync(Map.FromEntity(result), ct);
        }
    }
    
}
