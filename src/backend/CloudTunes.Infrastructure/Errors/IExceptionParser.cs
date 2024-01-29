using Microsoft.EntityFrameworkCore;

namespace CloudTunes.Infrastructure.Errors;

internal interface IExceptionParser
{
    bool IsAlreadyExists(DbUpdateException exception);
}
