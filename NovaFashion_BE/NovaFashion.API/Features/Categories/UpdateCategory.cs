using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
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
        public bool IsDeleted { get; set; }
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
            e.IsDeleted = r.IsDeleted;
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
                IsDeleted = e.IsDeleted,
                CreatedTime = e.CreatedTime
            };
        }
    }

    public class UpdateCategory(AppDbContext db) : Endpoint<UpdateCategoryRequest, CategoryDto, UpdateCategoryMapper>
    {
        public override void Configure()
        {
            Put("{id}");
            Group<CategoryGroup>();
            Roles(Role.Admin.ToString());
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
        {
            var category = await db.Categories
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (category == null)
            {
                ThrowError("Không tìm thấy danh mục", statusCode: 404);
            }

            if (req.IsDeleted == true && category.Products.Any(x => !x.IsDeleted))
            {
                AddError("Không thể inactive trạng thái của danh mục đang chứa sản phẩm");
            }

            ThrowIfAnyErrors();

            Map.UpdateEntity(req, category);

            db.Categories.Update(category);
            await db.SaveChangesAsync(ct);
            var response = Map.FromEntity(category);

            await Send.OkAsync(response, ct);
        }
    }
}
