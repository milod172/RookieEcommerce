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

        [JsonPropertyName("total_count")]       
        public int TotalCount { get; set; }

        [JsonPropertyName("page_size")]          
        public int PageSize { get; set; }

        [JsonPropertyName("page_number")]        
        public int PageNumber { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [JsonPropertyName("has_previous_page")]  
        public bool HasPreviousPage { get; set; }

        [JsonPropertyName("has_next_page")]     
        public bool HasNextPage { get; set; }
    }
}
