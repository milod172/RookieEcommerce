using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingRequest
    {
        [JsonPropertyName("comment")]
        [Required(ErrorMessage = "Nhận xét không được để trống")]
        public string Comment { get; set; } = string.Empty;
        [JsonPropertyName("rate")]
        [Range(1, 5, ErrorMessage = "Vui lòng đánh giá từ 1 đến 5 sao")]
        public int Rate { get; set; }
        [JsonPropertyName("order_id")]
        public Guid OrderId { get; set; }
    }
}
