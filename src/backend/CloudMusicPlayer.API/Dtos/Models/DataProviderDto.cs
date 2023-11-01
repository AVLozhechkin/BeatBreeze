using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record DataProviderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public ProviderTypes ProviderType { get; set; }
    public DateTimeOffset ConnectedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public IEnumerable<SongFileDto>? SongFiles { get; set; }

    public static DataProviderDto Create(DataProvider dataProvider)
    {
        return new DataProviderDto
        {
            Id = dataProvider.Id,
            UserId = dataProvider.UserId,
            Name = dataProvider.Name,
            ProviderType = dataProvider.ProviderType,
            ConnectedAt = dataProvider.ConnectedAt,
            UpdatedAt = dataProvider.UpdatedAt,
            SongFiles = dataProvider.SongFiles?.Select(SongFileDto.Create)
        };
    }
}
