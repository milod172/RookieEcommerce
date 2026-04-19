using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
   public record UpdateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryValidator : Validator<UpdateCategoryRequest>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
               .NotEmpty().WithMessage("Tên danh mục không được để trống")
               .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Mô tả không được vượt quá 255 ký tự");
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

    public class UpdateCategory(ICategoryRepository categoryRepository) : Endpoint<UpdateCategoryRequest, CategoryDto, UpdateCategoryMapper>
    {
        public override void Configure()
        {
            Put("{id}");
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
        {
            var entity = await categoryRepository.FindAsync(Route<Guid>("id"), ct);

            if (entity == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            Map.UpdateEntity(req, entity);

            await categoryRepository.UpdateAsync(entity, ct);

            var dto = Map.FromEntity(entity);

            await Send.OkAsync(dto, ct);
        }
    }
}
