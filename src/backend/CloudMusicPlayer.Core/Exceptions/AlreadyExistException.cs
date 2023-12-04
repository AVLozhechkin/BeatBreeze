namespace CloudMusicPlayer.Core.Exceptions;

public sealed class AlreadyExistException(string message) : DomainLayerException(message)
{
    public static AlreadyExistException Create<T>()
    {
        return new AlreadyExistException($"{nameof(T)} already exists");
    }
}
