using System.IO.Compression;
using System.Text.Json.Serialization;
using CloudMusicPlayer.API.Services;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.UnitOfWorks;
using CloudMusicPlayer.Infrastructure;
using CloudMusicPlayer.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Database dependencies
builder.Services.AddDbContexts("Data source = CloudMusicPlayer.db");

builder.Services.AddHttpClient();

builder.Services.AddRepositories();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddExternalDataProviders();

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();


builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<HistoryService>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});


// Authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "CloudMusicPlayer_User";
        options.Cookie.HttpOnly = false;
    })
    .AddCookie("External", options =>
    {
        options.Cookie.Name = "External";
        options.Cookie.HttpOnly = false;
    })
    .AddYandex(options =>
    {
        options.SignInScheme = "External";
        options.ClientId = builder.Configuration["yandex:clientId"]!;
        options.ClientSecret = builder.Configuration["yandex:clientSecret"]!;
        options.SaveTokens = true;
    }).AddDropbox(options =>
    {
        options.ClientId = builder.Configuration["dropbox:clientId"]!;
        options.ClientSecret = builder.Configuration["dropbox:clientSecret"]!;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseSpa(spa =>
{
    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
});


app.Run();
