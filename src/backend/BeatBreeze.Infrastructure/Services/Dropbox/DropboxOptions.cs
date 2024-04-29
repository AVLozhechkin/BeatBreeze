using System.ComponentModel.DataAnnotations;

namespace BeatBreeze.Infrastructure.Services.Dropbox;

internal sealed record DropboxOptions
{
    public const string SectionName = "dropbox";

    [Required]
    public required string ClientId { get; set; }

    [Required]
    public required string ClientSecret { get; set; }

    [Required]
    [Url]
    public required string FilesUrl { get; set; } = "https://api.dropboxapi.com/2/files";

    [Required]
    [Url]
    public required string OAuthUrl { get; set; } = "https://api.dropbox.com/oauth2/token";
}
