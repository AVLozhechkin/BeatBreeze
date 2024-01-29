using System.IO.Compression;
using System.Text.Json.Serialization;
using CloudTunes.API.Exceptions.Handlers;
using CloudTunes.API.Services;
using CloudTunes.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using OpenTelemetry.Metrics;

namespace CloudTunes.API.Utils;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.SetupCaching(configuration);
        services.SetupHttpClients();
        services.SetupExceptionHandling();
        services.SetupAuthService();
        services.SetupResponseCompression();
        services.SetupAuthentication(configuration);
        services.SetupAuthorization();
        services.SetupControllers();
        services.SetupMetrics();

        return services;
    }

    private static void SetupMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();
                builder.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
            });
    }

    private static void SetupControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                var enumConverter = new JsonStringEnumConverter();

                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                o.JsonSerializerOptions.Converters.Add(enumConverter);
            });
    }

    private static void SetupAuthService(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddScoped<AuthService>();
    }

    private static void SetupHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient();
    }

    private static void SetupCaching(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (Environment.GetEnvironmentVariable("Cache") == "InMemory")
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var connectionString = configuration.GetConnectionString("RedisCache");

                options.Configuration = connectionString;
            });
        }
    }

    private static void SetupExceptionHandling(this IServiceCollection services)
    {
        // Order is important. DomainExceptionHandler handles all domain exceptions
        services.AddExceptionHandler<DomainExceptionHandler>();
        // If exception is not handled by previous handler, than this one should return 500 for every exception
        services.AddExceptionHandler<UnknownExceptionHandler>();
    }

    private static void SetupAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "CloudTunes";
                options.Cookie.MaxAge = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = false;
            })
            .AddCookie("External", options =>
            {
                options.Cookie.Name = "CloudTunes_External";
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
