using System.ComponentModel.DataAnnotations;

namespace BeatBreeze.Infrastructure.Services.Yandex;

internal sealed record YandexOptions
{
    public const string SectionName = "yandex";

    [Required]
    public required string ClientId { get; set; }
    [Required]
    public required string ClientSecret { get; set; }
    [Required]
    [Url]
    public required string ResourceUrl { get; set; }
    [Required]
    [Url]
    public required string OAuthUrl { get; set; }
}
