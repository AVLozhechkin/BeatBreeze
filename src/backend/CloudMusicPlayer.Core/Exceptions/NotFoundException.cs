namespace CloudMusicPlayer.Core.Exceptions;

public sealed class NotFoundException : DomainLayerException
{
    private NotFoundException(string message, string resourceName, Guid? resourseId = null) : base(message)
    {
        if (resourseId.HasValue)
        {
            ResourseId = resourseId.Value;
        }

        ResourceName = resourceName;
    }

    public Guid? ResourseId { get; init; }
    public string ResourceName { get; }

    public static NotFoundException Create<T>()
    {
        string resourceName = typeof(T).Name;

        return new NotFoundException($"{resourceName} was not found", resourceName);
    }

    public static NotFoundException Create<T>(Guid resourceId)
    {
        string resourceName = typeof(T).Name;

        return new NotFoundException($"{resourceName} with ID: {resourceId} was not found", resourceName, resourceId);
    }
}
