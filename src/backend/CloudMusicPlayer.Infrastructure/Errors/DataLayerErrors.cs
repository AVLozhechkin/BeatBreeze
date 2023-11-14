using System.Net;
using CloudMusicPlayer.Core.Errors;

namespace CloudMusicPlayer.Infrastructure.Errors;

public static class DataLayerErrors
{
    public static class Database
    {
        // TODO check if it is better to put Exception message in the description field
        public static Error NoChangesMade() => new Error("database.noChanges", "No changes were saved");
        public static Error UpdateError() => new Error("database.update", "Something went wrong");
        public static Error DeleteError() => new Error("database.delete", "Something went wrong");
        public static Error GetError() => new Error("database.get", "Something went wrong");
        public static Error TransactionError() => new Error("database.transaction", "Something went wrong");
        public static Error NotFound() => new Error("database.notFound", "Something went wrong");
    }

    public static class Http
    {
        public static Error UnknownResponse() => new Error("http.unknownResponse", "Something went wrong");
        public static Error HttpError() => new Error("http.error", "Something went wrong");
        public static Error UnsuccessfulStatusCode(HttpStatusCode code) =>
            new Error("http.code", $"HTTP response has {code} status code.");
    }
}
