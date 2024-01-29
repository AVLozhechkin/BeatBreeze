namespace CloudMusicPlayer.Core.Exceptions;

public abstract class DomainLayerException : Exception
{
    protected DomainLayerException(string message) : base(message) { }
}
