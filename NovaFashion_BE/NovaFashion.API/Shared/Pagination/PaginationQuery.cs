using System.ComponentModel;
using System.Text.Json.Serialization;
using FastEndpoints;
using NovaFashion.API.Entities.Enum;

namespace NovaFashion.API.Shared.Pagination
{
    public class PaginationQuery
    {
        [QueryParam]
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [QueryParam]
        [DefaultValue(5)]
        public int PageSize { get; set; } 
        [QueryParam]
        [DefaultValue(FilterSort.Newest)]
        public FilterSort SortBy { get; set; } 
        [QueryParam]
        [DefaultValue(FilterStatus.Active)]
        public FilterStatus Status { get; set; }
        [QueryParam]
        public string? Search { get; set; }
    }
}
