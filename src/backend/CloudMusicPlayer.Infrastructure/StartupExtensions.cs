using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Core.UnitOfWorks;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.DataProviders.Dropbox;
using CloudMusicPlayer.Infrastructure.DataProviders.Yandex;
using CloudMusicPlayer.Infrastructure.Repositories;
using CloudMusicPlayer.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMusicPlayer.Infrastructure;

public static class StartupExtensions
{
    public static void AddDbContexts(this IServiceCollection services, string appString)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlite(appString));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IDataProviderRepository, DataProviderRepository>();

        services.AddScoped<IPlaylistRepository, PlaylistRepository>();

        services.AddScoped<IPlaylistItemRepository, PlaylistItemRepository>();

        services.AddScoped<ISongFileRepository, SongFileRepository>();

        services.AddScoped<IHistoryRepository, HistoryRepository>();

        services.AddScoped<IHistoryItemRepository, HistoryItemRepository>();

    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddExternalDataProviders(this IServiceCollection services, IConfigurationManager configuration)
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
