using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingDto
    {
        public Guid Id { get; set; }
        public string RatingBy { get; set; } = string.Empty;
        [JsonPropertyName("rating_date")]
        public string RatingDate { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
    }
}
