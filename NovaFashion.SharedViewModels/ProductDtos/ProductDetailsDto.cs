using System;
using System.Collections.Generic;
using System.Text;

namespace NovaFashion.SharedViewModels.ProductDtos
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public string? Details { get; set; } = string.Empty;
        public int TotalQuantity { get; set; } = 0;
        public string Sku { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
