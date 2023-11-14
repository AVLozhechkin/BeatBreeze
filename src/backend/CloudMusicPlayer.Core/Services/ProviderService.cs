using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;

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
        var providersResult = await _unitOfWork.DataProviderRepository.GetAllByUserIdAsync(userId);

        return providersResult;
    }

    public async Task<Result<DataProvider?>> GetDataProvider(Guid providerId, Guid userId)
    {
        var providerResult = await _unitOfWork.DataProviderRepository.GetAsync(providerId, true);

        if (providerResult.IsFailure)
        {
            return providerResult;
        }

        if (providerResult.Value is not null && providerResult.Value.UserId != userId)
        {
            return Result.Failure<DataProvider?>(DomainLayerErrors.NotTheOwner());
        }

        return providerResult;
    }

    public async Task<Result<string>> GetSongFileUrl(Guid songFileId, Guid userId)
    {
        var songFileResult = await _unitOfWork.SongFileRepository.GetById(songFileId, true);

        if (songFileResult.IsFailure)
        {
            return Result.Failure<string>(songFileResult.Error);
        }

        if (songFileResult.Value?.DataProvider is null)
        {
            return Result.Failure<string>(DomainLayerErrors.NotFound());
        }

        if (songFileResult.Value.DataProvider.UserId != userId)
        {
            return Result.Failure<string>(DomainLayerErrors.NotTheOwner());
        }

        var songFile = songFileResult.Value;

        var dictionary = new Dictionary<string, string>();

        var externalProvider = _externalProviderServices
            .FirstOrDefault(ep
                => ep.CanBeExecuted(songFile.DataProvider.ProviderType));

        if (externalProvider is null)
        {
            return Result.Failure<string>(DomainLayerErrors.ExternalProviderNotFound());
        }

        Task<Result> updateDataProvider = null!;

        // If token is expired
        if (songFile.DataProvider.AccessTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            var apiTokenResult = await externalProvider.GetApiToken(songFile.DataProvider.RefreshToken);

            if (apiTokenResult.IsFailure)
            {
                return Result.Failure<string>(apiTokenResult.Error);
            }

            songFile.DataProvider.AccessToken = apiTokenResult.Value.Token;
            songFile.DataProvider.AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(apiTokenResult.Value.ExpiresIn);

            updateDataProvider = _unitOfWork.DataProviderRepository.UpdateAsync(songFile.DataProvider, true);
        }

        // TODO Review task awaiting here

        var urlResult = await externalProvider.GetSongFileUrl(songFile, songFile.DataProvider);

        if (urlResult.IsFailure)
        {
            return urlResult; // TODO "Can't fetch song's url"
        }

        if (updateDataProvider is null)
        {
            return Result.Success(urlResult.Value);
        }


        // TODO should I return failure on update? Because if we successfully got the link then token is okay. Review is needed.
        var updateResult = await updateDataProvider;
        /*if (updateResult.IsFailure)
        {
            return Result.Failure<string>(updateResult.Error);
        }*/

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
        // TODO Check the existing provider

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
            return Result.Failure<DataProvider>(songFilesResult.Error);
        }

        await _unitOfWork.SongFileRepository.AddRangeAsync(songFilesResult.Value);

        var commitResult = await _unitOfWork.CommitAsync();

        if (commitResult.IsFailure)
        {
            return Result.Failure<DataProvider>(commitResult.Error);
        }

        return dataProviderResult;
    }

    public async Task<Result<DataProvider>> UpdateDataProvider(Guid providerId, Guid userId)
    {
        var providerResult = await _unitOfWork.DataProviderRepository.GetAsync(providerId, true, true);

        if (providerResult.IsFailure)
        {
            return providerResult;
        }

        var provider = providerResult.Value;

        if (provider is null)
        {
            return Result.Failure<DataProvider>(DomainLayerErrors.NotFound());
        }

        if (provider.UserId != userId)
        {
            return Result.Failure<DataProvider>(DomainLayerErrors.NotTheOwner());
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

        return providerResult;
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
