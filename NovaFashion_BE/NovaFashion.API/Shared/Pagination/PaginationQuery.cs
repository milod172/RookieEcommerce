using System.ComponentModel;
using System.Text.Json.Serialization;
using FastEndpoints;

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
        [DefaultValue("Id desc")]
        public string? SortBy { get; set; } 
        [QueryParam]
        [DefaultValue(false)]
        public bool IncludeDeleted { get; set; }
    }
}
