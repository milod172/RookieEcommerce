using System.ComponentModel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.OrdersTable;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.API.Features.ProductRatings
{
    public class GetProductRatingInProductRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid ProductId { get; set; }

        [QueryParam]
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [QueryParam]
        [DefaultValue(5)]
        public int PageSize { get; set; }
    }

    public class GetProductRatingInProductMapper : Mapper<GetProductRatingInProductRequest, PaginationList<ProductRatingDto>, PaginationList<ProductRating>>
    {
        public ProductRatingDto MapToDto(ProductRating e)
        {
            return new ProductRatingDto
            {
                Comment = e.Comment,
                Id = e.Id,
                RatingDate = e.CreatedTime.ToString("dd/MM/yyyy"),
                Rate = e.Rate,
                RatingBy = e.CreatedBy ?? ""
            };
        }

        public override PaginationList<ProductRatingDto> FromEntity(PaginationList<ProductRating> e)
        {
            var dtos = e.Items.Select(x => MapToDto(x)).ToList();

            return new PaginationList<ProductRatingDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }

    public class GetProductRatingInProduct(AppDbContext db): Endpoint<GetProductRatingInProductRequest, PaginationList<ProductRatingDto>, GetProductRatingInProductMapper>
    {
        public override void Configure()
        {
            Get("{id}/rating");
            AllowAnonymous();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(GetProductRatingInProductRequest req, CancellationToken ct)
        {
            var query = db.ProductRatings
                .AsNoTracking()
                .Where(x => x.ProductId == req.ProductId);

            var pageResultEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);
            var pageResultDtos = Map.FromEntity(pageResultEntities);
            await Send.OkAsync(pageResultDtos, ct);
        }
    }
}
