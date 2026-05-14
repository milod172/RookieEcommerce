using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("rating_by")]
        public string RatingBy { get; set; } = string.Empty;
        [JsonPropertyName("rating_date")]
        public string RatingDate { get; set; } = string.Empty;
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;
        [JsonPropertyName("rate")]
        public int Rate { get; set; }
    }
}
