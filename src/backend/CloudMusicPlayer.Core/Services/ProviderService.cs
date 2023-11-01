using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Services;

public class ProviderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<IExternalProviderService> _externalProviderServices;

    public ProviderService(IUnitOfWork unitOfWork,
        IEnumerable<IExternalProviderService> externalProviderServices
        )
    {
        _unitOfWork = unitOfWork;
        _externalProviderServices = externalProviderServices;
    }

    public async Task<Result<List<DataProvider>>> GetAllProvidersByUserId(Guid userId)
    {
        var providers = await _unitOfWork.DataProviderRepository.GetAllByUserIdAsync(userId, true);

        return Result.Success(providers);
    }

    public async Task<Result<DataProvider?>> GetDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetAsync(providerId, true);

        if (provider is not null && provider.UserId != userId)
        {
            return Result.Failure<DataProvider?>("User is not the owner of the DataProvider");
        }

        return Result.Success(provider);
    }

    public async Task<Result<DataProvider>> AddDataProvider(
        ProviderTypes providerType,
        Guid userId,
        string name,
        string apiToken,
        string refreshToken,
        string expiresAt)
    {
        var dataProviderResult = DataProvider.Create(name, userId, providerType, apiToken, refreshToken, expiresAt);

        if (dataProviderResult.IsFailure)
        {
            return Result.Failure<DataProvider>(dataProviderResult.Error);
        }

        await _unitOfWork.DataProviderRepository.AddAsync(dataProviderResult.Value);

        var externalProviderService = _externalProviderServices
            .First(s =>
                s.CanBeExecuted(providerType));

        var songFiles = await externalProviderService.GetSongFiles(dataProviderResult.Value);

        await _unitOfWork.SongFileRepository.AddRangeAsync(songFiles);

        var commitResult = await _unitOfWork.CommitAsync();

        if (commitResult.IsFailure)
        {
            return Result.Failure<DataProvider>(commitResult.Error);
        }

        return Result.Success(dataProviderResult.Value);
    }

    public async Task<Result<DataProvider>> UpdateDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetAsync(providerId, true, true);

        if (provider is null)
        {
            return Result.Failure<DataProvider>("DataProvider was not found");
        }

        if (provider.UserId != userId)
        {
            return Result.Failure<DataProvider>("User is not the owner of the DataProvider");
        }

        var externalProviderService = _externalProviderServices
            .First(s =>
                s.CanBeExecuted(provider.ProviderType));

        var songFiles = await externalProviderService.GetSongFiles(provider);

        UpdateSongFiles(provider, songFiles);
        provider.UpdatedAt = DateTimeOffset.UtcNow;

        await _unitOfWork.DataProviderRepository.UpdateAsync(provider);

        var result = await _unitOfWork.CommitAsync();

        if (result.IsFailure)
        {
            return Result.Failure<DataProvider>(result.Error);
        }

        return Result.Success(provider);
    }

    public async Task<Result> RemoveDataProvider(Guid providerId, Guid userId)
    {

        var result = await _unitOfWork.DataProviderRepository.RemoveAsync(providerId, userId, true);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        return Result.Success();
    }

    private void UpdateSongFiles(DataProvider provider, IReadOnlyCollection<SongFile> newSongs)
    {
        var newSongsDictionary = new Dictionary<string, SongFile>(newSongs.Count);

        foreach (var newSong in newSongs)
        {
            newSongsDictionary.Add(newSong.FileId, newSong);
        }

        foreach (var oldSong in provider.SongFiles)
        {
            if (!newSongsDictionary.TryGetValue(oldSong.FileId, out SongFile? value))
            {
                _unitOfWork.SongFileRepository.RemoveAsync(oldSong);
            }
            else
            {
                oldSong.Url = value.Url;
                newSongsDictionary.Remove(oldSong.FileId);
            }
        }

        _unitOfWork.SongFileRepository.AddRangeAsync(newSongsDictionary.Values);
    }
}
