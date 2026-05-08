using System.Text.Json.Serialization;

namespace NovaFashion.SharedViewModels.AuthenticationDtos
{
    public record LoginRequest(string Email, string Password);

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
