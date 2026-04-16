using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.API.Persistence;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }

    public class CreateCategoryValidator : Validator<CreateCategoryRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
    public class CreateCategory : Endpoint<CreateCategoryRequest, CategoryDto>
    {
        private readonly AppDbContext _context;
        public CreateCategory(AppDbContext context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Post("");
            Group<CategoryGroup>();
            AllowAnonymous();
            //RequireAuthorization()
            Description(x => x
                .WithName("CreateCategory")
                .Produces<CategoryDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest));
        }

        public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
        {
            var category = new Category
            {
                CategoryName = req.CategoryName,
                Description = req.Description,
                ParentCategoryId = req.ParentCategoryId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId
            };
            await Send.CreatedAtAsync("", null, categoryDto, cancellation: ct);
        }
    }
}
