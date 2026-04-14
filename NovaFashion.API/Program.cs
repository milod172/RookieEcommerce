using NovaFashion.API;
using NovaFashion.API.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructure(appSettings.ConnectionStrings.DefaultConnection);

// Register CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "NovaFashion API Reference";
        options.Theme = ScalarTheme.BluePlanet;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.ShowSidebar = true;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
