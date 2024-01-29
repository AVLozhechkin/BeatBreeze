using CloudTunes.Core.Interfaces;
using CloudTunes.Core.Interfaces.Repositories;
using CloudTunes.Infrastructure.Database;
using CloudTunes.Infrastructure.Errors;
using CloudTunes.Infrastructure.Repositories;
using CloudTunes.Infrastructure.Services.Dropbox;
using CloudTunes.Infrastructure.Services.Yandex;
using CloudTunes.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudTunes.Infrastructure.Utils;

public static class DependencyInjectionExtensions
{
    private const string DbConnectionStringName = "PostgresDb";

    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddUnitOfWork();
        services.AddExternalDataProviders();

        return services;
    }

    private static void AddDbContext(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (Environment.GetEnvironmentVariable("Database") == "SQLite")
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data source = CloudTunes.db"));
            services.AddSingleton<IExceptionParser, SqliteExceptionParser>();
        }
        else
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString(DbConnectionStringName)));
            services.AddSingleton<IExceptionParser, PostgresExceptionParser>();
        }
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataProviderRepository, DataProviderRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IPlaylistItemRepository, PlaylistItemRepository>();
        services.AddScoped<IMusicFileRepository, MusicFileRepository>();
    }

    private static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddExternalDataProviders(this IServiceCollection services)
    {
        services.AddOptions<DropboxOptions>()
            .BindConfiguration(DropboxOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<YandexOptions>()
            .BindConfiguration(YandexOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IExternalProviderService, YandexDiskProvider>();
        services.AddScoped<IExternalProviderService, DropboxProvider>();
    }
}
