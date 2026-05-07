namespace NovaFashion.SharedViewModels.AuthenticationDtos
{
    public record LoginRequest(string Email, string Password);

    public record TokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime AccessExpiry { get; init; }
        public DateTime RefreshExpiry { get; init; }
        public string UserId { get; init; } = string.Empty;
    }

    public record RefreshRequest(string UserId, string RefreshToken);
}
