using CloudMusicPlayer.Core.Errors;

namespace CloudMusicPlayer.Core.Models;

public record DataProvider
{
    public Guid Id { get; init; }

    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public ProviderTypes ProviderType { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTimeOffset AddedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; init; } = string.Empty;
    public IReadOnlyList<SongFile> SongFiles { get; init; } = null!;

    public static Result<DataProvider> Create(
        string name,
        Guid userId,
        ProviderTypes providerType,
        string apiToken,
        string refreshToken,
        string expiresAt)
    {
        if (string.IsNullOrWhiteSpace(apiToken))
        {
            return Result.Failure<DataProvider>(DomainLayerErrors.DataProvider.ApiTokenIsNullOrEmpty());
        }

        var currentDate = DateTimeOffset.UtcNow;
        if (!DateTimeOffset.TryParse(expiresAt, out var expires))
        {
            return Result.Failure<DataProvider>(DomainLayerErrors.DataProvider.ApiTokenExpireTokenCantBeParsed());
        }

        var provider = new DataProvider()
        {
            UserId = userId,
            ProviderType = providerType,
            Name = name,
            AddedAt = currentDate,
            UpdatedAt = currentDate,
            AccessToken = apiToken,
            AccessTokenExpiresAt = expires,
            RefreshToken = refreshToken,
        };

        return Result.Success(provider);
    }
}
