using System.Text.Json;
using NovaFashion.SharedViewModels.AuthenticationDtos;

namespace NovaFashion.CustomerSite.Services.Auth
{
    public class AuthApiClient(HttpClient httpClient, ILogger<AuthApiClient> logger)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<TokenResponse?> LoginAsync(string email, string password)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(
                    "/api/auth/login",
                    new LoginRequest(email, password)
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    logger.LogWarning("Login failed for {Email}. Status: {Status}. Response: {Error}",
                        email, response.StatusCode, error);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception during login for {Email}", email);
                return null;
            }
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(
                    "/api/auth/refresh-token",
                    new RefreshRequest(userId, refreshToken)
                );

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Refresh token failed for UserId: {UserId}", userId);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception during token refresh for UserId: {UserId}", userId);
                return null;
            }
        }
    }
}
