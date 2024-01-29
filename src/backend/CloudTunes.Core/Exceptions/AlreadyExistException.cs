namespace CloudTunes.Core.Exceptions;

public sealed class AlreadyExistException : DomainLayerException
{
    private AlreadyExistException(string message, string resourceName) : base(message)
    {

        ResourceName = resourceName;
    }

    public string ResourceName { get; }

    public static AlreadyExistException Create<T>()
    {
        var resourceName = typeof(T).Name;

        return new AlreadyExistException($"{resourceName} already exists", resourceName);
    }

    public static AlreadyExistException Create(string resourceName)
    {
        return new AlreadyExistException($"{resourceName} already exists", resourceName);
    }
}
