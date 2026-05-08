using System.Text.Json;
using NovaFashion.CustomerSite;
using NovaFashion.CustomerSite.Configuration;
using NovaFashion.CustomerSite.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddNovaFashionAuthentication(appSettings.JwtSettings.TokenExpiryInMinutes);

//Register API clients 
builder.Services.AddApiClients(appSettings.ApiSettings.BaseUrl);

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
