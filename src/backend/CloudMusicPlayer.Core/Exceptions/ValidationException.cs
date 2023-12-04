namespace CloudMusicPlayer.Core.Exceptions;

public sealed class ValidationException(string message) : DomainLayerException(message);
