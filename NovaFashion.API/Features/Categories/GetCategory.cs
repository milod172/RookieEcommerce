using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Entities;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{

    public class GetCategoryMapper : Mapper<PaginationQuery, PaginationList<CategoryDto>, PaginationList<Category>>
    {
        public CategoryDto MapToDto(Category e)
        {
            return new CategoryDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime
            };
        }

        public override PaginationList<CategoryDto> FromEntity(PaginationList<Category> e)
        {
            var dtos = e.Items.Select(MapToDto).ToList();

            return new PaginationList<CategoryDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }


    public class GetCategory(ICategoryRepository categoryRepository): Endpoint<PaginationQuery, PaginationList<CategoryDto>, GetCategoryMapper>
    {
        public override void Configure()
        {
            Get("");   
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var result = await categoryRepository.GetPagedCategoriesAsync(req, ct);
            var dto = Map.FromEntity(result);

            await Send.OkAsync(dto, ct);
        }
    }
}
