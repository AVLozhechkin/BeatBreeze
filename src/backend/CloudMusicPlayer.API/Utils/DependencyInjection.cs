using System.IO.Compression;
using CloudMusicPlayer.API.Exceptions;
using CloudMusicPlayer.API.Services;
using CloudMusicPlayer.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;

namespace CloudMusicPlayer.API.Utils;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpClient();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddExceptionHandler<ExceptionHandler>();

        services.AddScoped<AuthService>();

        services.SetupResponseCompression();

        services.SetupAuthentication(configuration);

        services.SetupAuthorization();

        return services;
    }

    private static void SetupAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "CloudMusicPlayer";
                options.Cookie.HttpOnly = false;
            })
            .AddCookie("External", options =>
            {
                options.Cookie.Name = "CloudMusicPlayer_External";
            })
            .AddYandex(options =>
            {
                options.SignInScheme = "External";
                options.ClientId = configuration["yandex:clientId"]!;
                options.ClientSecret = configuration["yandex:clientSecret"]!;
                options.SaveTokens = true;
            }).AddDropbox(options =>
            {
                options.SignInScheme = "External";
                options.ClientId = configuration["dropbox:clientId"]!;
                options.ClientSecret = configuration["dropbox:clientSecret"]!;
                options.SaveTokens = true;
                options.AccessType = "offline";
            });
    }

    private static void SetupAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
    }

    private static void SetupResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

    }
}
