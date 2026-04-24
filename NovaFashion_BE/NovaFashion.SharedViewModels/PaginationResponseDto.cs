using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels
{
    
    public class PaginationResponseDto<T>
    {
        public PaginationResponseDto()
        {
        }

        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = [];

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => PageNumber > 1;

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
