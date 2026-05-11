using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using NovaFashion.SharedViewModels.CartDtos;

namespace NovaFashion.SharedViewModels.OrderDtos
{
    public class OrderCheckoutRequest
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("phone_number")]
        public string Phone { get; set; }
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }
        [JsonPropertyName("items")]
        public List<CartItemRequest> Items { get; set; }
    }
}
