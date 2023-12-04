using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Repositories;
using CloudMusicPlayer.Infrastructure.Services.Dropbox;
using CloudMusicPlayer.Infrastructure.Services.Yandex;
using CloudMusicPlayer.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMusicPlayer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, string appString)
    {
        AddDbContext(services, appString);
        AddRepositories(services);
        AddUnitOfWork(services);
        AddExternalDataProviders(services);

        return services;
    }

    private static void AddDbContext(IServiceCollection services, string appString)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlite(appString));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IDataProviderRepository, DataProviderRepository>();

        services.AddScoped<IPlaylistRepository, PlaylistRepository>();

        services.AddScoped<IPlaylistItemRepository, PlaylistItemRepository>();

        services.AddScoped<ISongFileRepository, SongFileRepository>();

        services.AddScoped<IHistoryRepository, HistoryRepository>();

        services.AddScoped<IHistoryItemRepository, HistoryItemRepository>();

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
