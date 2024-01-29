using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
using CloudMusicPlayer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.UnitOfWorks;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _applicationContext;
    private readonly IExceptionParser _exceptionParser;
    private IDataProviderRepository? _dataProviderRepository;
    private IPlaylistRepository? _playlistRepository;
    private IMusicFileRepository? _musicFileRepository;
    private IUserRepository? _userRepository;
    private IPlaylistItemRepository? _playlistItemRepository;

    public UnitOfWork(ApplicationContext applicationContext, IExceptionParser exceptionParser)
    {
        _applicationContext = applicationContext;
        _exceptionParser = exceptionParser;
    }

    public IDataProviderRepository DataProviderRepository =>
        _dataProviderRepository ??= new DataProviderRepository(_applicationContext);
    public IPlaylistItemRepository PlaylistItemRepository =>
        _playlistItemRepository ??= new PlaylistItemRepository(_applicationContext);
    public IPlaylistRepository PlaylistRepository =>
        _playlistRepository ??= new PlaylistRepository(_applicationContext, _exceptionParser);
    public IMusicFileRepository MusicFileRepository =>
        _musicFileRepository ??= new MusicFileRepository(_applicationContext);
    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_applicationContext, _exceptionParser);

    public async Task CommitAsync()
    {
        await using var dbContextTransaction = await _applicationContext.Database.BeginTransactionAsync();

        try
        {
            await _applicationContext.SaveChangesAsync();
            await dbContextTransaction.CommitAsync();
        }
        catch(DbUpdateException ex) when (_exceptionParser.IsAlreadyExists(ex))
        {
            await dbContextTransaction.RollbackAsync();

            var failedEntryType = ex.Entries[0].CurrentValues.EntityType.ClrType.Name;

            throw AlreadyExistException.Create(failedEntryType);
        }
        catch (Exception)
        {
            await dbContextTransaction.RollbackAsync();
            throw;
        }
    }
}
