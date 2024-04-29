using Microsoft.EntityFrameworkCore;

namespace BeatBreeze.Infrastructure.Errors;

internal interface IExceptionParser
{
    bool IsAlreadyExists(DbUpdateException exception);
}
