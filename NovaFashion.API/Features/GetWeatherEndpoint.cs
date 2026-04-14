using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace NovaFashion.API.Features
{
    public class GetWeatherEndpoint : EndpointBaseSync
    .WithoutRequest
    .WithResult<List<WeatherForecast>>
    {
        [HttpGet("/weather")]
        public override List<WeatherForecast> Handle()
        {
            var rng = new Random();

            var data = Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = "Sample data"
                })
                .ToList();

            return data;
        }
    }
}
