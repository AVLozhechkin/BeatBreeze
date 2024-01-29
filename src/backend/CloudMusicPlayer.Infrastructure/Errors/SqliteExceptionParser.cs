using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Errors;
internal sealed class SqliteExceptionParser : IExceptionParser
{
    public bool IsAlreadyExists(DbUpdateException exception)
    {
        // because 2067 code in SQLite means unique index constraint
        if (exception.InnerException is SqliteException sqliteException && sqliteException.SqliteExtendedErrorCode == 2067)
        {
            return true;
        }

        return false;
    }
}
