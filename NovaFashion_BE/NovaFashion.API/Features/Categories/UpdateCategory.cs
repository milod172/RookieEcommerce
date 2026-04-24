using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
   public record UpdateCategoryRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryValidator : Validator<UpdateCategoryRequest>
    {
        public const string CategoryNameRequired = "Tên danh mục không được để trống";
        public const string CategoryNameTooLong = "Tên danh mục không được vượt quá 100 ký tự";
        public const string DescriptionTooLong = "Mô tả không được vượt quá 255 ký tự";

        public UpdateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage(CategoryNameRequired)
            .MaximumLength(100)
            .WithMessage(CategoryNameTooLong);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(DescriptionTooLong);
        }
    }

    public class UpdateCategoryMapper : Mapper<UpdateCategoryRequest, CategoryDto, Category>
    {
        public override Category UpdateEntity(UpdateCategoryRequest r, Category e)
        {
            e.CategoryName = r.CategoryName;
            e.Description = r.Description;
            e.ParentCategoryId = r.ParentCategoryId;

            return e;
        }

        public override CategoryDto FromEntity(Category e)
        {
            return new CategoryDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                ParentCategoryId = e.ParentCategoryId,
                CreatedTime = e.CreatedTime
            };
        }
    }

    public class UpdateCategory(AppDbContext db) : Endpoint<UpdateCategoryRequest, CategoryDto, UpdateCategoryMapper>
    {
        public override void Configure()
        {
            Put("{id}");
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
        {
            var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (category == null)
            {
                ThrowError("Không tìm thấy danh mục", statusCode: 404);
            }

            Map.UpdateEntity(req, category);

            db.Categories.Update(category);
            await db.SaveChangesAsync(ct);
            var response = Map.FromEntity(category);

            await Send.OkAsync(response, ct);
        }
    }
}
