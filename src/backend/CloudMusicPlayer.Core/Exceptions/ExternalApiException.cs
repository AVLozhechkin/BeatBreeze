namespace CloudMusicPlayer.Core.Exceptions;

public sealed class ExternalApiException(string message) : DomainLayerException(message);
