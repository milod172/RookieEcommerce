using FastEndpoints;
using FluentValidation;
using NovaFashion.API.Shared.Pagination;

namespace NovaFashion.API.Shared.Validators
{
    public class PaginationQueryValidator : Validator<PaginationQuery>
    {
        public PaginationQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageNumber must be more than or equal to 1");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(5, 20)           
                .WithMessage("PageSize must be between 5 to 20 items");

        }
    }
}
