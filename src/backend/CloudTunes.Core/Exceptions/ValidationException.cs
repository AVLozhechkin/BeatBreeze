namespace CloudTunes.Core.Exceptions;

public sealed class ValidationException : DomainLayerException
{
    private ValidationException(string message, string property, string description) : base(message)
    {
        Property = property;
        Description = description;
    }

    public string Property { get; init; }
    public string Description { get; init; }

    public static ValidationException Create(string property, string description)
    {
        return new ValidationException($"An error occured when validating ${property}: ${description}", property, description);
    }
}
