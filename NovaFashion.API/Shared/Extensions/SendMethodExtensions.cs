using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace NovaFashion.API.Shared.Extensions
{
    public static class SendMethodExtensions
    {
        public static Task CreatedAsync<TResponse>(
        this IResponseSender sender,
        TResponse? response = default,
        CancellationToken ct = default)
        {
           
            return sender.HttpContext.Response.SendAsync(
                response,
                StatusCodes.Status201Created,
                cancellation: ct);
        }
    }
}
