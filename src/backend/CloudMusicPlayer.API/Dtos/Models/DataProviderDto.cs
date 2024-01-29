using System.Text.Json.Serialization;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record DataProviderDto
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public ProviderTypes ProviderType { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<MusicFileDto>? MusicFiles { get; set; }


    public static DataProviderDto Create(DataProvider dataProvider, bool includeFiles = false)
    {
        ArgumentNullException.ThrowIfNull(dataProvider);

        return new DataProviderDto
        {
            Id = dataProvider.Id,
            UserId = dataProvider.UserId,
            Name = dataProvider.Name,
            ProviderType = dataProvider.ProviderType,
            AddedAt = dataProvider.AddedAt,
            UpdatedAt = dataProvider.UpdatedAt,
            MusicFiles = includeFiles ? dataProvider.MusicFiles?.Select(MusicFileDto.Create) : null
        };
    }
}
