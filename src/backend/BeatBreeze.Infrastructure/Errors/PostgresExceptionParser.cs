using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BeatBreeze.Infrastructure.Errors;
internal sealed class PostgresExceptionParser : IExceptionParser
{
    public bool IsAlreadyExists(DbUpdateException exception)
    {
        // because 23505 code in PostgreSQL means unique index constraint
        if (exception.InnerException is PostgresException pgException && pgException.SqlState == "23505")
        {
            return true;
        }

        return false;
    }
}
