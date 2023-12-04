namespace CloudMusicPlayer.Core.Services.Encryption;

internal record EncryptionOptions
{
    public const string SectionName = "Encryption";

    public required string Secret { get; set; }
}
