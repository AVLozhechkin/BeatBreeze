using System.Text.Json.Serialization;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record DataProviderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProviderTypes ProviderType { get; set; }
    public DateTimeOffset AddedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<SongFileDto>? SongFiles { get; set; }

    public static DataProviderDto Create(DataProvider dataProvider)
    {
        return new DataProviderDto
        {
            Id = dataProvider.Id,
            UserId = dataProvider.UserId,
            Name = dataProvider.Name,
            ProviderType = dataProvider.ProviderType,
            AddedAt = dataProvider.AddedAt,
            UpdatedAt = dataProvider.UpdatedAt,
            SongFiles = dataProvider.SongFiles?.Select(SongFileDto.Create)
        };
    }
}
