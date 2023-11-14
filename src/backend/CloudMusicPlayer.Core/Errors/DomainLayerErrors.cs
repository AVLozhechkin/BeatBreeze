namespace CloudMusicPlayer.Core.Errors;

public static class DomainLayerErrors
{
    public static class DataProvider
    {
        public static Error ApiTokenIsNullOrEmpty() =>
            new Error("domain.dataProvider.token.null", "Api token is null or whitespace");
        public static Error ApiTokenExpireTokenCantBeParsed() =>
            new Error("domain.dataProvider.token.expireTime", "Api token expiring time can't be parsed");
    }

    public static class Playlist
    {
        public static Error PlaylistNameIsNullOrWhitespace() => new Error("domain.playlist.name", "Playlist name must not be a null or whitespace");
        public static Error NoPlaylistItem() => new Error("domain.playlist.playlistItems", "There is no such element in playlist");
    }

    public static class User
    {
        public static Error InvalidEmail() => new Error("domain.user.email", "Email is incorrect");
        public static Error InvalidName() => new Error("domain.user.name", "Name must have at least 2 characters and less than 30");
    }

    public static Error NotTheOwner() => new Error("domain.unauthorized", "User is not the owner of the resource");
    public static Error NotFound() => new Error("domain.notfound", "Resource not found");
    public static Error ExternalProviderNotFound() => new Error("domain.externalProvider.notFound", "External provider service not found");
}
