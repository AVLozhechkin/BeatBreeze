using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.Services.Encryption;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMusicPlayer.Core.Utils;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCoreLayer(this IServiceCollection services)
    {
        services.AddServices();

        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddOptions<EncryptionOptions>()
            .BindConfiguration(EncryptionOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IEncryptionService, EncryptionService>();

        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
    }
}
