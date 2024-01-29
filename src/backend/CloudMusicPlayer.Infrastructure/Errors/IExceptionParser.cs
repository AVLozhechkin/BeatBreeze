using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Errors;

internal interface IExceptionParser
{
    bool IsAlreadyExists(DbUpdateException exception);
}
