using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderAdminDto
    {
        public Guid Id { get; set; }
        public string OrderDate { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
