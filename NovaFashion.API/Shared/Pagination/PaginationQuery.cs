using System.Text.Json.Serialization;
using FastEndpoints;

namespace NovaFashion.API.Shared.Pagination
{
    public class PaginationQuery
    {
        
        public int PageNumber { get; set; } = 1;
       
        public int PageSize { get; set; } = 10;
        
        public string? SortBy { get; set; } = "Id desc";
    }
}
