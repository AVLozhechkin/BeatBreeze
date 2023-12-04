using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.Extensions.Logging;

namespace CloudMusicPlayer.Core.Services;

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

    public async Task<List<DataProvider>> GetAllProvidersByUserId(Guid userId)
    {
        return await _unitOfWork.DataProviderRepository.GetAllByUserIdAsync(userId, false, true);
    }

    public async Task<DataProvider> GetDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetByIdAsync(providerId, true, true);

        if (provider is null)
        {
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        if (provider.UserId != userId)
        {
            _logger.LogWarning("User ({userId}) requested a data provider ({providerId}), " +
                               "but it has another owner ({userId})", userId, providerId, provider.UserId);
            throw NotTheOwnerException.Create<DataProvider>();
        }

        return provider;
    }

    public async Task<string> GetSongFileUrl(Guid songFileId, Guid userId)
    {
        var songFile = await _unitOfWork.SongFileRepository.GetById(songFileId, true, true);

        if (songFile is null)
        {
            throw NotFoundException.Create<SongFile>(songFileId);
        }

        if (songFile.DataProvider.UserId != userId)
        {
            throw NotTheOwnerException.Create<SongFile>(songFileId);
        }

        var externalProviderService = _externalProviderServices
            .Single(ep=> ep.CanBeExecuted(songFile.DataProvider.ProviderType));

        Task? updateDataProvider = null;

        if (songFile.DataProvider.AccessTokenExpiration < DateTimeOffset.UtcNow)
        {
            var accessToken = await externalProviderService.GetAccessToken(songFile.DataProvider.RefreshToken);

            var encryptedAccessToken = _encryptionService.Encrypt(accessToken.Token);

            var expirationDate = DateTimeOffset.UtcNow.AddSeconds(accessToken.ExpiresIn);

            songFile.DataProvider.UpdateAccessToken(encryptedAccessToken, expirationDate);

            updateDataProvider = _unitOfWork.DataProviderRepository.UpdateAsync(songFile.DataProvider, true);
        }

        string url = await externalProviderService.GetSongFileUrl(songFile, songFile.DataProvider);

        if (updateDataProvider is not null)
        {
            await updateDataProvider;
            _logger.LogInformation("Updated access token for provider ({providerId})", songFile.DataProvider.Id);
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
            return;
        }

        var encryptedToken = _encryptionService.Encrypt(accessToken);
        var encryptedRefreshToken = _encryptionService.Encrypt(refreshToken);
        var tokenExpiresAt = DateTimeOffset.Parse(expiresAt);

        var dataProvider = new DataProvider(name, userId, providerType, encryptedToken, encryptedRefreshToken, tokenExpiresAt);

        await _unitOfWork.DataProviderRepository.AddAsync(dataProvider, false);

        var externalProviderService = _externalProviderServices
            .Single(s => s.CanBeExecuted(providerType));

        var songFiles = await externalProviderService.GetSongFiles(dataProvider);

        await _unitOfWork.SongFileRepository.AddRangeAsync(songFiles, false);

        await _unitOfWork.CommitAsync();

        _logger.LogInformation("User ({userId}) added a provider ({providerId}) with ({songFilesCount}) songfiles",
            userId, dataProvider.Id, songFiles.Count);
    }

    public async Task<DataProvider> UpdateDataProvider(Guid providerId, Guid userId)
    {
        var provider = await _unitOfWork.DataProviderRepository.GetByIdAsync(providerId, true, false);

        if (provider is null)
        {
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        if (provider.UserId != userId)
        {
            throw NotTheOwnerException.Create<DataProvider>(providerId);
        }

        var externalProviderService = _externalProviderServices
            .Single(s => s.CanBeExecuted(provider.ProviderType));

        // TODO Update token using refresh
        bool tokenUpdateRequires = provider.AccessTokenExpiration < DateTimeOffset.UtcNow;

        if (tokenUpdateRequires)
        {
            var accessToken = await externalProviderService.GetAccessToken(provider.RefreshToken);

            var encryptedAccessToken = _encryptionService.Encrypt(accessToken.Token);

            var expirationDate = DateTimeOffset.UtcNow.AddSeconds(accessToken.ExpiresIn);

            provider.UpdateAccessToken(encryptedAccessToken, expirationDate);

            await _unitOfWork.DataProviderRepository.UpdateAsync(provider, false);
        }

        var songFiles = await externalProviderService.GetSongFiles(provider);

        UpdateSongFiles(provider, songFiles);

        await _unitOfWork.CommitAsync();

        if (tokenUpdateRequires)
        {
            _logger.LogInformation("User ({userId}) updated songFiles for provider ({providerId}) and updated " +
                                   "an access token for it",userId, providerId);
        }
        else
        {
            _logger.LogInformation("User ({userId}) updated songFiles for provider ({providerId})",
                userId, providerId);
        }

        return provider;
    }

    public async Task RemoveDataProvider(Guid providerId, Guid userId)
    {
        await _unitOfWork.DataProviderRepository.RemoveAsync(providerId, userId, true);
        _logger.LogInformation("User ({userId}) removed provider ({providerId})",userId, providerId);
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
            if (!newSongsDictionary.Remove(oldSong.FileId, out SongFile? _))
            {
                _unitOfWork.SongFileRepository.RemoveAsync(oldSong, false);
            }
        }

        _unitOfWork.SongFileRepository.AddRangeAsync(newSongsDictionary.Values,false);

        provider.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
