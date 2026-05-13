using System.Security.Claims;
using System.Text.Json.Serialization;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductRatingDtos;

namespace NovaFashion.API.Features.ProductRatings
{
    public class CreateProductRatingRequest
    {
        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public Guid OrderId { get; set; }
        [JsonIgnore]
        public OrderStatus OrderStatus { get; set; }
    }

    public class CreateProductRatingValidator : Validator<CreateProductRatingRequest>
    {
        public const string CommentRequired = "Nội dung đánh giá không được để trống";
        public const string CommentTooLong = "Nội dung đánh giá không được vượt quá 1000 ký tự";
        public const string RateInvalid = "Vui lòng đánh giá theo thang từ 1 đến 5 sao";

        public CreateProductRatingValidator()
        {
            RuleFor(x => x.Rate)
                .InclusiveBetween(1, 5)
                .WithMessage(RateInvalid);

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage(CommentRequired)
                .MaximumLength(1000)
                .WithMessage(CommentTooLong);
        }
    }

    public class CreateProductRatingMapper : Mapper<CreateProductRatingRequest, ProductRatingDto, ProductRating>
    {
        public override ProductRating ToEntity(CreateProductRatingRequest r)
        {
            return new ProductRating
            {
                Comment = r.Comment,
                Rate = r.Rate,
                OrderId = r.OrderId,
            };
        }
    }

    public class CreateProductRating(AppDbContext db) : Endpoint<CreateProductRatingRequest, ProductRatingDto, CreateProductRatingMapper>
    {
        public override void Configure()
        {
            Post("");
            Roles(Role.Customer.ToString());
            Group<ProductRatingGroup>();
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(CreateProductRatingRequest req, CancellationToken ct)
        {
            var existRating = await db.ProductRatings.AnyAsync(x => x.OrderId == req.OrderId, ct);
            if (existRating)
            {
                ThrowError("Đơn hàng đã được đánh giá", statusCode: 400);
                return;
            }

            var currentUserId = User.FindFirstValue("sub");

            var order = await db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(x => x.Id == req.OrderId && x.CustomerId == currentUserId, ct);
            if(order == null)
            {
                ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
                return;
            }


            if(order.OrderStatus != OrderStatus.Completed)
            {
                AddError(x => x.OrderStatus, "Chỉ được đánh giá khi hoàn tất thanh toán đơn hàng");
                
            }
            ThrowIfAnyErrors();

            var productRating = Map.ToEntity(req);
            productRating.CustomerId = order.CustomerId!;
            productRating.ProductId = order.OrderItems.FirstOrDefault()!.ProductVariant!.ProductId;
            productRating.CreatedBy = $"{order.Customer!.FirstName} {order.Customer.LastName}";

            db.ProductRatings.Add(productRating);
            await db.SaveChangesAsync(ct);

            var response = new ProductRatingDto
            {
                Id = productRating.Id,
                RatingBy = productRating.CreatedBy,
                Comment = productRating.Comment,
                RatingDate = productRating.CreatedTime.ToString("dd/MM/yyyy"),
                Rate = productRating.Rate
            }; ;

            await Send.OkAsync(response, ct);
        }
    }

}
