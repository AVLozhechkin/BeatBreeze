namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex;

public record YandexOptions
{
    public const string SectionName = "yandex";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
