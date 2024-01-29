namespace CloudTunes.Core.Services.Encryption;

internal sealed record EncryptionOptions
{
    public const string SectionName = "Encryption";

    public required string Secret { get; set; }
}
