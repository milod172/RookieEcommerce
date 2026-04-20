using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public record CreateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }

    public class CreateCategoryValidator : Validator<CreateCategoryRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Mô tả không được vượt quá 255 ký tự");
        }
    }

    public class CreateCategoryMapper : Mapper<CreateCategoryRequest, CategoryDto, Category>
    {
        public override Category ToEntity(CreateCategoryRequest r)
        {
            return new Category
            {
                CategoryName = r.CategoryName,
                Description = r.Description,
                ParentCategoryId = r.ParentCategoryId
            };
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

    public class CreateCategory(AppDbContext db) : Endpoint<CreateCategoryRequest, CategoryDto, CreateCategoryMapper>
    {
        public override void Configure()
        {
            Post("");
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
        {
            var entity = Map.ToEntity(req);

            db.Categories.Add(entity);                    
            await db.SaveChangesAsync(ct);

            var response = Map.FromEntity(entity);

            await Send.CreatedAsync(response, ct);
        }
    }
}
