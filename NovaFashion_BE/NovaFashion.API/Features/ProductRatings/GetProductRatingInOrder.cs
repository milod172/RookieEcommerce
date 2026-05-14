using System.ComponentModel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.OrdersTable;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.API.Features.ProductRatings
{
    public class GetProductRatingInOrderRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid OrderId { get; set; }

    }

    public class GetProductRatingInOrderMapper : Mapper<GetProductRatingInOrderRequest, ProductRatingDto, ProductRating>
    {
        public override ProductRatingDto FromEntity(ProductRating e)
        {
            return new ProductRatingDto { 
                Comment = e.Comment,
                Id = e.Id,
                RatingDate = e.CreatedTime.ToString("dd/MM/yyyy"),
                Rate = e.Rate,
                RatingBy = e.CreatedBy ?? ""
            };
        }
    }

    public class GetProductRatingInOrder(AppDbContext db) : Endpoint<GetProductRatingInOrderRequest, ProductRatingDto, GetProductRatingInOrderMapper>
    {
        public override void Configure()
        {
            Get("{id}/rating");
            Group<OrderGroup>();
        }

        public override async Task HandleAsync(GetProductRatingInOrderRequest req, CancellationToken ct)
        {
            var rating = await db.ProductRatings
                .SingleOrDefaultAsync(x => x.OrderId == req.OrderId, ct);

            if (rating == null)
            {
                await Send.OkAsync(null, ct);
                return;
            }

            await Send.OkAsync(Map.FromEntity(rating), ct);
        }
    }
}
