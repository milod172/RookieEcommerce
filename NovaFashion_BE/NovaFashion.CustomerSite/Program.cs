using System.Text.Json;
using NovaFashion.CustomerSite;
using NovaFashion.CustomerSite.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddNovaFashionAuthentication();

//Register API clients 
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]!;
builder.Services.AddApiClients(apiBaseUrl);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseMiddleware<StatusCodeMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenRefreshMiddleware>();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
