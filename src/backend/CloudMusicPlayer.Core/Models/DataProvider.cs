using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Models;

public record DataProvider
{
    [Key]
    public Guid Id { get; init; }

    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public ProviderTypes ProviderType { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ApiToken { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset ConnectedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<SongFile> SongFiles { get; init; } = null!;

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
            return Result.Failure<DataProvider>("Api token is null or whitespace");
        }

        var currentDate = DateTimeOffset.UtcNow;
        if (!DateTimeOffset.TryParse(expiresAt, out var expires))
        {
            return Result.Failure<DataProvider>("Api token expiring time can't be parsed");
        }


        var provider = new DataProvider()
        {
            UserId = userId,
            ProviderType = providerType,
            Name = name,
            ConnectedAt = currentDate,
            UpdatedAt = currentDate,
            ApiToken = apiToken,
            RefreshToken = refreshToken,
            ExpiresAt = expires
        };

        return Result.Success(provider);
    }
}
