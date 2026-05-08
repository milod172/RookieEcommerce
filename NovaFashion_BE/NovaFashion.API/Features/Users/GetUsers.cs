using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.UserDtos;

namespace NovaFashion.API.Features.Users
{

    public class GetUsersMapper : Mapper<PaginationQuery, PaginationList<GetUserDto>, PaginationList<ApplicationUser>>
    {
        public override PaginationList<GetUserDto> FromEntity(PaginationList<ApplicationUser> e)
        {
            var dtos = e.Items.Select(x => new GetUserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FirstName + " " + x.LastName,
                PhoneNumber = x.PhoneNumber,
                IsActive = x.IsActive
            }).ToList();

            return new PaginationList<GetUserDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }
    public class GetUsersEndPoint(AppDbContext db) : Endpoint<PaginationQuery, PaginationList<GetUserDto>, GetUsersMapper>
    {
        public override void Configure()
        {
            Get("");
            Roles(Role.Admin.ToString());
            Validator<PaginationQueryValidator>();
            Group<UserGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");

            var query = db.Users
                .Where(u => u.Id != currentUserId)
                .AsNoTracking()
                .ApplyStatusFilter(req.Status)
                .ApplySortFilter(req.SortBy);

            var pageResultEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);
            var pageResultDtos = Map.FromEntity(pageResultEntities);
            await Send.OkAsync(pageResultDtos, ct);
        }
    }

    internal static class GetUsersQueryExtensions
    {
        internal static IQueryable<ApplicationUser> ApplyStatusFilter(
            this IQueryable<ApplicationUser> query,
            FilterStatus status)
            => status switch
            {
                FilterStatus.Active => query.Where(p => p.IsActive),
                FilterStatus.Inactive => query.Where(p => !p.IsActive),
                _ => query
            };

        internal static IQueryable<ApplicationUser> ApplySortFilter(
            this IQueryable<ApplicationUser> query,
            FilterSort sortBy)
            => sortBy switch
            {
                FilterSort.Oldest => query.OrderBy(p => p.CreatedDate),
                FilterSort.Newest => query.OrderByDescending(p => p.CreatedDate),
                FilterSort.IdAsc => query.OrderBy(p => p.Id),
                FilterSort.IdDesc => query.OrderByDescending(p => p.Id),
                FilterSort.NameAsc => query.OrderBy(p => p.UserName),
                FilterSort.NameDesc => query.OrderByDescending(p => p.UserName),
                _ => query
            };
    }
}
