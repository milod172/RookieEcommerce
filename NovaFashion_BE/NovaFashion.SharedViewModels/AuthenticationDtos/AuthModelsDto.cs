using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.AuthenticationDtos
{
    public record LoginRequest(string Email, string Password);

    public record RegisterRequest
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        [JsonPropertyName("confirm_password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = string.Empty;
        [JsonPropertyName("access_expiry")]
        public DateTime AccessExpiry { get; init; }
        [JsonPropertyName("refresh_expiry")]
        public DateTime RefreshExpiry { get; init; }
        [JsonPropertyName("user_id")]
        public string UserId { get; init; } = string.Empty;
    }

    public record RefreshRequest(string UserId, string RefreshToken);
}
