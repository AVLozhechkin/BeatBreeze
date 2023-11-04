namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox;

public record DropboxOptions
{
    public const string SectionName = "dropbox";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
