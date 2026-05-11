using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.CategoryDtos;
using NovaFashion.SharedViewModels.OrderDtos;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.OrdersTable
{
    public class GetOrdersForCustomerMapper : Mapper<PaginationQuery, PaginationList<OrderDto>, PaginationList<Orders>>
    {
        public OrderDto MapToDto(Orders e) => new()
        {
            Id = e.Id,
            OrderStatus = e.OrderStatus.ToString(),
            TotalAmount = e.TotalAmount,
            Items = e.OrderItems.Select(x => new OrderItemDto
            {
              ProductName = x.ProductVariant.Product.ProductName,
              ImageUrl = x.ProductVariant!.Product!.ProductImages
                 .FirstOrDefault(x => x.IsPrimary)?.ImageUrl ?? string.Empty,
              Size = x.ProductVariant.Size.ToString(),
              Quantity = x.Quantity,
              TotalPrice = x.UnitPrice * x.Quantity
            }).ToList()       
        };

        public override PaginationList<OrderDto> FromEntity(PaginationList<Orders> e)
        {
            var dtos = e.Items.Select(x => MapToDto(x)).ToList();

            return new PaginationList<OrderDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }

    public class GetOrdersForCustomer(AppDbContext db) : Endpoint<PaginationQuery, PaginationList<OrderDto>, GetOrdersForCustomerMapper>
    {
        public override void Configure()
        {
            Get("/my-orders");
            Roles(Role.Customer.ToString());
            Validator<PaginationQueryValidator>();
            Group<OrderGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");
            if(currentUserId == null)
            {
                ThrowError("Vui lòng đăng nhập lại", statusCode: 401);
                return;
            }

            var orders = db.Orders.AsNoTracking();

            var pageResultEntities = await orders.PaginateAsync(
                req.PageNumber,
                req.PageSize,
                q => q
                    .Include(p => p.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(pv => pv.Product)
                                .ThenInclude(p => p.ProductImages),    
                ct);


            var pageResultDtos = Map.FromEntity(pageResultEntities);
            await Send.OkAsync(pageResultDtos, ct);
        }
    }
}
