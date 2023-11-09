namespace CloudMusicPlayer.Core;

public static class DomainErrors
{
    public const string NotTheOwner = "User is not the owner of the resource";
    public const string NotFound = "Resource not found";
    public const string CantBeCreated = "Resource can't be created";
}
