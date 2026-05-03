
using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.Json;
using NovaFashion.API;
using NovaFashion.API.Configuration;
using NovaFashion.API.Infrastructure.Seed;
using NovaFashion_API;
using NSwag;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

builder.Services.AddControllers();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddFastEndpoints();

builder.Services
    .AddApiServices(builder.Configuration)
    .AddInfrastructure(appSettings.ConnectionStrings.DefaultConnection)
    .AddApplicationServices()
    .AddJwtAuthentication(appSettings.JwtSettings);


builder.Services.SwaggerDocument(opt =>
{
    opt.SerializerSettings = x =>
    {
        x.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        x.Converters.Add(new JsonStringEnumConverter());
    };

    opt.DocumentSettings = s =>
    {
        s.Title = "Nova Fashion API";
        s.Version = "v1";
    };
});
// Register CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


   
var app = builder.Build();

await app.SeedDatabaseAsync(appSettings.AdminSettings);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen(); 
}

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    c.Serializer.Options.AddSerializerContextsFromNovaFashion_API();
    c.Binding.ReflectionCache.AddFromNovaFashionAPI();
    c.Endpoints.RoutePrefix = "api";
});


app.Run();
