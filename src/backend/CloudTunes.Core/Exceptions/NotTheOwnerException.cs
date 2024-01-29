namespace CloudTunes.Core.Exceptions;

public sealed class NotTheOwnerException : DomainLayerException
{
    private NotTheOwnerException(string message, string resourceName, Guid? resourseId = null) : base(message)
    {
        if (resourseId.HasValue)
        {
            ResourseId = resourseId.Value;
        }

        ResourceName = resourceName;
    }

    public Guid? ResourseId { get; init; }
    public string ResourceName { get; }

    public static NotTheOwnerException Create<T>()
    {
        string resourceName = typeof(T).Name;

        return new NotTheOwnerException($"User is not the owner of the {resourceName}", resourceName);
    }

    public static NotTheOwnerException Create<T>(Guid resourceId)
    {
        string resourceName = typeof(T).Name;

        return new NotTheOwnerException($"User is not the owner of the {resourceName} with ID: {resourceId}", resourceName, resourceId);
    }
}

