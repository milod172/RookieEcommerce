using System.ComponentModel;
using System.Text.Json.Serialization;
using FastEndpoints;

namespace NovaFashion.API.Shared.Pagination
{
    public class PaginationQuery
    {
        [QueryParam]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        [QueryParam]
        [DefaultValue(5)]
        public int PageSize { get; set; } = 5;
        [QueryParam]
        [DefaultValue("Id desc")]
        public string? SortBy { get; set; } = "Id desc";
    }
}
