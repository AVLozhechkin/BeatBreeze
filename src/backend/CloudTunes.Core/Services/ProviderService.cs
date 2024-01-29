using CloudTunes.Core.Enums;
using CloudTunes.Core.Exceptions;
using CloudTunes.Core.Interfaces;
using CloudTunes.Core.Models;
using Microsoft.Extensions.Logging;

namespace CloudTunes.Core.Services;

internal sealed class ProviderService : IProviderService
{
    private readonly ILogger<ProviderService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;
    private readonly IEnumerable<IExternalProviderService> _externalProviderServices;

    public ProviderService(
        ILogger<ProviderService> logger,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService,
        IEnumerable<IExternalProviderService> externalProviderServices)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
        _externalProviderServices = externalProviderServices;
    }

    public async Task<List<DataProvider>> GetProvidersByUserId(Guid userId, bool includeFiles)
    {
        return await _unitOfWork.DataProviderRepository.GetByUserIdAsync(userId, includeFiles, true);
    }

    public async Task<DataProvider> GetDataProviderById(Guid providerId, Guid userId, bool includeFiles)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetByIdAsync(providerId, includeFiles, true);

        if (provider is null)
        {
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        if (provider.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) requested a data provider ({ProviderId}), " +
                               "but it has another owner ({UserId})", userId, providerId, provider.UserId);
            throw NotTheOwnerException.Create<DataProvider>();
        }

        return provider;
    }

    public async Task<IEnumerable<MusicFile>> GetSongsForDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetByIdAsync(providerId, true, true);

        if (provider is null)
        {
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        if (provider.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) requested a data provider ({ProviderId}), " +
                               "but it has another owner ({UserId})", userId, providerId, provider.UserId);
            throw NotTheOwnerException.Create<DataProvider>();
        }

        return provider.MusicFiles!; //! - because we fetch DataProvider with the music provider
    }

    public async Task<string> GetMusicFileUrl(Guid musicFileId, Guid userId)
    {
        var musicFile = await _unitOfWork.MusicFileRepository.GetById(musicFileId, true, true);

        if (musicFile is null)
        {
            throw NotFoundException.Create<MusicFile>(musicFileId);
        }

        if (musicFile.DataProvider.UserId != userId)
        {
            throw NotTheOwnerException.Create<MusicFile>(musicFileId);
        }

        var externalProviderService = _externalProviderServices
            .Single(ep => ep.CanBeExecuted(musicFile.DataProvider.ProviderType));

        Task? updateDataProvider = null;

        if (musicFile.DataProvider.AccessTokenExpiration < DateTimeOffset.UtcNow)
        {
            var accessToken = await externalProviderService.GetAccessToken(musicFile.DataProvider);

            var encryptedAccessToken = _encryptionService.Encrypt(accessToken.Token);

            var expirationDate = DateTimeOffset.UtcNow.AddSeconds(accessToken.ExpiresIn);

            musicFile.DataProvider.UpdateAccessToken(encryptedAccessToken, expirationDate);

            updateDataProvider = _unitOfWork.DataProviderRepository.UpdateAsync(musicFile.DataProvider, true);
        }

        string url = await externalProviderService.GetMusicFileUrl(musicFile, musicFile.DataProvider);

        if (updateDataProvider is not null)
        {
            await updateDataProvider;
            _logger.LogInformation("Updated access token for provider ({ProviderId})", musicFile.DataProvider.Id);
        }

        return url;
    }

    public async Task AddDataProvider(
        ProviderTypes providerType,
        Guid userId,
        string name,
        string accessToken,
        string refreshToken,
        string expiresAt)
    {
        var existingProvider = await _unitOfWork
            .DataProviderRepository.GetByTypeAndName(providerType, name, userId, true);

        if (existingProvider is not null)
        {
            _logger.LogWarning("User ({UserId}) tried to add an existing provider ({ProviderId})", userId, existingProvider.Id);
            return;
        }

        var encryptedToken = _encryptionService.Encrypt(accessToken);
        var encryptedRefreshToken = _encryptionService.Encrypt(refreshToken);
        var tokenExpiresAt = DateTimeOffset.Parse(expiresAt);

        var dataProvider = new DataProvider(name, userId, providerType, encryptedToken, encryptedRefreshToken, tokenExpiresAt);

        await _unitOfWork.DataProviderRepository.AddAsync(dataProvider, false);

        var externalProviderService = _externalProviderServices
            .Single(s => s.CanBeExecuted(providerType));

        var musicFiles = await externalProviderService.GetMusicFiles(dataProvider);

        await _unitOfWork.MusicFileRepository.AddRangeAsync(musicFiles, false);

        await _unitOfWork.CommitAsync();

        _logger.LogInformation("User ({UserId}) added a provider ({ProviderId}) with ({MusicFilesCount}) musicfiles",
            userId, dataProvider.Id, musicFiles.Count);
    }

    public async Task<DataProvider> UpdateDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetByIdAsync(providerId, true, false);

        if (provider is null)
        {
            _logger.LogWarning("User ({UserId}) tried to update non-existing provider ({ProviderId})", userId, providerId);
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        if (provider.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) tried to update provider ({ProviderId}), but it has another owner ({UserId})", userId, providerId, provider.UserId);
            throw NotTheOwnerException.Create<DataProvider>(providerId);
        }

        var externalProviderService = _externalProviderServices
            .Single(s => s.CanBeExecuted(provider.ProviderType));

        // TODO Update token using refresh
        bool tokenUpdateRequires = provider.AccessTokenExpiration < DateTimeOffset.UtcNow;

        if (tokenUpdateRequires)
        {
            var accessToken = await externalProviderService.GetAccessToken(provider);

            var encryptedAccessToken = _encryptionService.Encrypt(accessToken.Token);

            var expirationDate = DateTimeOffset.UtcNow.AddSeconds(accessToken.ExpiresIn);

            provider.UpdateAccessToken(encryptedAccessToken, expirationDate);

            await _unitOfWork.DataProviderRepository.UpdateAsync(provider, false);
        }

        var musicFiles = await externalProviderService.GetMusicFiles(provider);

        UpdateMusicFiles(provider, musicFiles);

        await _unitOfWork.CommitAsync();

        if (tokenUpdateRequires)
        {
            _logger.LogInformation("User ({UserId}) updated musicFiles for provider ({ProviderId}) and updated " +
                                   "an access token for it", userId, providerId);
        }
        else
        {
            _logger.LogInformation("User ({UserId}) updated musicFiles for provider ({ProviderId})",
                userId, providerId);
        }

        return provider;
    }

    public async Task RemoveDataProvider(Guid providerId, Guid userId)
    {
        await _unitOfWork.DataProviderRepository.RemoveAsync(providerId, userId, true);
        _logger.LogInformation("User ({UserId}) removed provider ({ProviderId})", userId, providerId);
    }

    private void UpdateMusicFiles(DataProvider provider, IReadOnlyCollection<MusicFile> newSongs)
    {
        if (provider.MusicFiles is null)
        {
            throw new ArgumentNullException(nameof(provider), "Provider must be passed with MusicFiles property");
        }

        var newSongsDictionary = new Dictionary<string, MusicFile>(newSongs.Count);

        foreach (var newSong in newSongs)
        {
            newSongsDictionary.Add(newSong.FileId, newSong);
        }

        foreach (var oldSong in provider.MusicFiles)
        {
            if (!newSongsDictionary.Remove(oldSong.FileId, out MusicFile? _))
            {
                _unitOfWork.MusicFileRepository.RemoveAsync(oldSong, false);
            }
        }

        _unitOfWork.MusicFileRepository.AddRangeAsync(newSongsDictionary.Values, false);

        provider.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
