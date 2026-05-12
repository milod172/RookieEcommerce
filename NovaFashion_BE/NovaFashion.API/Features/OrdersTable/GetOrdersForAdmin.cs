using System.ComponentModel;
using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.OrderDtos;

namespace NovaFashion.API.Features.OrdersTable
{
    public class GetOrdersForAdminQuery : PaginationQuery
    {
        [QueryParam]
        [DefaultValue(OrderStatus.All)]
        public OrderStatus OrderStatus { get; set; }
    }

    public class GetOrdersForAdminMapper : Mapper<GetOrdersForAdminQuery, PaginationList<OrderAdminDto>, PaginationList<Orders>>
    {
        public OrderAdminDto MapToDto(Orders o) => new()
        {
            Id = o.Id,
            OrderBy = $"{o.Customer?.FirstName} {o.Customer?.LastName}",
            TotalAmount = o.TotalAmount,
            OrderDate = o.CreatedTime.ToString("dd/MM/yyyy"),
            OrderStatus = o.OrderStatus.ToString()
        };

        public override PaginationList<OrderAdminDto> FromEntity(PaginationList<Orders> o)
        {
            var dtos = o.Items.Select(x => MapToDto(x)).ToList();

            return new PaginationList<OrderAdminDto>(
                dtos,
                o.TotalCount,
                o.PageNumber,
                o.PageSize
            );
        }
    }

    public class GetOrdersForAdmin(AppDbContext db): Endpoint<GetOrdersForAdminQuery, PaginationList<OrderAdminDto>, GetOrdersForAdminMapper>
    {
        public override void Configure()
        {
            Get("");
            Roles(Role.Admin.ToString());
            Validator<PaginationQueryValidator>();
            Group<OrderGroup>();
        }

        public override async Task HandleAsync(GetOrdersForAdminQuery req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");
            if (currentUserId == null)
            {
                ThrowError("Vui lòng đăng nhập lại", statusCode: 401);
                return;
            }

            var orders = db.Orders
                .AsNoTracking()
                .ApplyOrderStatusFilter(req.OrderStatus)
                .ApplySearch(req.Search)
                .ApplySortFilter(req.SortBy);

            var pageResultEntities = await orders.PaginateAsync(
                req.PageNumber,
                req.PageSize,
                q => q
                    .Include(p => p.Customer),ct);


            var pageResultDtos = Map.FromEntity(pageResultEntities);
            await Send.OkAsync(pageResultDtos, ct);
        }
    }

    internal static class GetOrderQueryExtensions
    {
        internal static IQueryable<Orders> ApplySearch(
          this IQueryable<Orders> query,
          string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var keyword = search.Trim().ToLower();

            return query.Where(p =>
               (p.Customer.FirstName + " " + p.Customer.LastName).ToLower().Contains(keyword) ||
                p.Id.ToString().Contains(keyword)
            );
        }


        internal static IQueryable<Orders> ApplyStatusFilter(
            this IQueryable<Orders> query,
            FilterStatus status)
            => status switch
            {
                FilterStatus.Active => query.Where(p => !p.IsDeleted),
                FilterStatus.Inactive => query.Where(p => p.IsDeleted),
                _ => query
            };

        internal static IQueryable<Orders> ApplyOrderStatusFilter(
            this IQueryable<Orders> query,
            OrderStatus sortBy)
            => sortBy switch
            {
                OrderStatus.All => query,

                OrderStatus.Pending =>
                    query.Where(p => p.OrderStatus == OrderStatus.Pending),

                OrderStatus.Paid =>
                    query.Where(p => p.OrderStatus == OrderStatus.Paid),

                OrderStatus.Completed =>
                    query.Where(p => p.OrderStatus == OrderStatus.Completed),

                _ => query
            };

        internal static IQueryable<Orders> ApplySortFilter(
            this IQueryable<Orders> query,
            FilterSort sortBy)
            => sortBy switch
            {
                FilterSort.Oldest => query.OrderBy(p => p.CreatedTime),
                FilterSort.Newest => query.OrderByDescending(p => p.CreatedTime), 
                FilterSort.PriceAsc => query.OrderBy(p => p.TotalAmount),
                FilterSort.PriceDesc => query.OrderByDescending(p => p.TotalAmount),
                _ => query
            };

        
    }
}
