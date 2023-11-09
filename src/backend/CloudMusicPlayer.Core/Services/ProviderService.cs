using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Services;

public sealed class ProviderService
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
        var providers = await _unitOfWork.DataProviderRepository.GetAllByUserIdAsync(userId);

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

    public async Task<Result<string>> GetSongFileUrl(Guid songFileId, Guid userId)
    {
        var songFile = await _unitOfWork.SongFileRepository.GetById(songFileId, true);

        if (songFile?.DataProvider is null)
        {
            return Result.Failure<string>("SongFile not found");
        }

        if (songFile.DataProvider.UserId != userId)
        {
            return Result.Failure<string>("User is not the owner of the SongFile");
        }

        var externalProvider = _externalProviderServices
            .FirstOrDefault(ep
                => ep.CanBeExecuted(songFile.DataProvider.ProviderType));

        if (externalProvider is null)
        {
            return Result.Failure<string>("External provider service not found");
        }

        Task<Result> updateDataProvider = null!;

        // If token is expired
        if (songFile.DataProvider.AccessTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            var apiTokenResult = await externalProvider.GetApiToken(songFile.DataProvider.RefreshToken);

            if (apiTokenResult.IsFailure)
            {
                return Result.Failure<string>("Api token is expired. Can't fetch the new one.");
            }

            songFile.DataProvider.AccessToken = apiTokenResult.Value.Token;
            songFile.DataProvider.AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(apiTokenResult.Value.ExpiresIn);

            updateDataProvider = _unitOfWork.DataProviderRepository.UpdateAsync(songFile.DataProvider, true);
        }

        var urlResult = await externalProvider.GetSongFileUrl(songFile, songFile.DataProvider);

        if (urlResult.IsFailure)
        {
            return Result.Failure<string>("Can't fetch song's url");
        }

        if (updateDataProvider is not null)
        {
            var updateResult = await updateDataProvider;
            if (updateResult.IsFailure)
            {
                return Result.Failure<string>("Data provider can't be updated");
            }
        }

        return Result.Success(urlResult.Value);
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

        var songFilesResult = await externalProviderService.GetSongFiles(dataProviderResult.Value);

        if (songFilesResult.IsFailure)
        {
            return Result.Failure<DataProvider>("An error occured when fetching song files");
        }

        await _unitOfWork.SongFileRepository.AddRangeAsync(songFilesResult.Value);

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

        var songFilesResult = await externalProviderService.GetSongFiles(provider);

        UpdateSongFiles(provider, songFilesResult.Value);
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
                newSongsDictionary.Remove(oldSong.FileId);
            }
        }

        _unitOfWork.SongFileRepository.AddRangeAsync(newSongsDictionary.Values);
    }
}
