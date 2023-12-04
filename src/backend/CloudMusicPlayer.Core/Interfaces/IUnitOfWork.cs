using CloudMusicPlayer.Core.Interfaces.Repositories;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public IHistoryRepository HistoryRepository { get; }
    public IHistoryItemRepository HistoryItemRepository { get; }
    public IPlaylistItemRepository PlaylistItemRepository { get; }
    public IDataProviderRepository DataProviderRepository { get; }
    public ISongFileRepository SongFileRepository { get; }
    public IPlaylistRepository PlaylistRepository { get; }
    public Task CommitAsync();
}
