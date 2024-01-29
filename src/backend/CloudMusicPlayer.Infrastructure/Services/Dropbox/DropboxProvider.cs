using System.Net.Http.Json;
using System.Text.Json;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Services.Dropbox.Args;
using CloudMusicPlayer.Infrastructure.Services.Dropbox.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.Services.Dropbox;

internal sealed class DropboxProvider: IExternalProviderService
{
    private const string ListFolderPath = "list_folder";
    private const string ListFolderContinuePath = "list_folder/continue";
    private const string TemporaryLinkPath = "get_temporary_link";
    private const ushort Limit = 2000;

    public DropboxProvider(HttpClient httpClient,
        IEncryptionService encryptionService,
        ILogger<DropboxProvider> logger,
        IOptions<DropboxOptions> options)
    {
        _httpClient = httpClient;
        _encryptionService = encryptionService;
        _logger = logger;
        _options = options.Value;
    }


    private readonly DropboxOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<DropboxProvider> _logger;

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Dropbox;
    }

    public async Task<IReadOnlyList<MusicFile>> GetMusicFiles(DataProvider provider)
    {
        _logger.LogInformation_GetMusicFiles_Start(ProviderTypes.Dropbox, provider.Id);

        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var defaultBody = new ListFolderArg
        {
            Limit = Limit,
            Path = "", // root of the disk
            Recursive = true
        };

        var uri = Path.Combine(_options.FilesUrl, ListFolderPath);

        var response = await _httpClient.PostAsJsonAsync(uri, defaultBody);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetMusicFiles_BadResponse(ProviderTypes.Dropbox, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "Dropbox is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var listFolderResult = JsonSerializer.Deserialize<ListFolderResult>(body);

        if (listFolderResult is null)
        {
            _logger.LogError_GetMusicFiles_CantSerialize(ProviderTypes.Dropbox, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "ListFolder response was not properly deserialized");
        }

        var musicFiles = new List<MusicFile>(listFolderResult.Entries.Count);

        FillMusicFilesWithEntries(musicFiles, listFolderResult.Entries, provider);

        while (listFolderResult.HasMore)
        {
            _logger.LogInformation_GetMusicFiles_Remaining(ProviderTypes.Dropbox, provider.Id);

            var cursorBody = new ListFolderContinueArg() { Cursor = listFolderResult.Cursor };

            uri = Path.Combine(_options.FilesUrl, ListFolderContinuePath);

            response = await _httpClient.PostAsJsonAsync(uri, cursorBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError_GetMusicFiles_BadResponse(ProviderTypes.Dropbox, provider.Id, response.StatusCode);
                throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "Dropbox is unavailable. Please, come back later.");
            }

            body = await response.Content.ReadAsStringAsync();

            listFolderResult = JsonSerializer.Deserialize<ListFolderResult>(body);

            if (listFolderResult is null)
            {
                _logger.LogError_GetMusicFiles_CantSerialize(ProviderTypes.Dropbox, provider.Id, response.StatusCode, body);
                throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "ListFolder response was not properly deserialized");
            }

            FillMusicFilesWithEntries(musicFiles, listFolderResult.Entries, provider);
        }

        _logger.LogInformation_GetMusicFiles_Result(musicFiles.Count, ProviderTypes.Dropbox, provider.Id);

        return musicFiles;
    }

    public async Task<string> GetMusicFileUrl(MusicFile musicFile, DataProvider provider)
    {
        _logger.LogInformation_GetMusicFileUrl_Start(musicFile.Id, ProviderTypes.Dropbox, provider.Id);

        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var requestBody = new TemporaryLinkArg
        {
            Path = musicFile.Path
        };

        var uri = Path.Combine(_options.FilesUrl, TemporaryLinkPath);

        var response = await _httpClient.PostAsJsonAsync(uri, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetMusicFileUrl_BadResponse(musicFile.Id, ProviderTypes.Dropbox, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "Dropbox is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var temporaryLink = JsonSerializer.Deserialize<TemporaryLinkResult>(body);

        if (temporaryLink is null)
        {
            _logger.LogError_GetMusicFileUrl_CantSerialize(musicFile.Id, ProviderTypes.Dropbox, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "Link response was not properly deserialized");
        }

        _logger.LogInformation_GetMusicFileUrl_Result(musicFile.Id, ProviderTypes.Dropbox, provider.Id);
        return temporaryLink.Link;
    }

    public async Task<AccessToken> GetAccessToken(DataProvider provider)
    {
        _logger.LogInformation_GetAccessToken_Start(ProviderTypes.Dropbox, provider.Id);

        var decryptedRefreshToken = _encryptionService.Decrypt(provider.RefreshToken);

        var args = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", decryptedRefreshToken },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
        };

        var uri = _options.OAuthUrl;

        using var requestBody = new FormUrlEncodedContent(args);

        var response = await _httpClient.PostAsync(uri, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetAccessToken_BadResponse(ProviderTypes.Dropbox, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "Dropbox is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var accessToken = JsonSerializer.Deserialize<AccessToken>(body);

        if (accessToken is null)
        {
            _logger.LogError_GetAccessToken_CantSerialize(ProviderTypes.Dropbox, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Dropbox, response.StatusCode, "AccessToken response was not properly deserialized");
        }

        _logger.LogInformation_GetAccessToken_Result(ProviderTypes.Dropbox, provider.Id);

        return accessToken;
    }

    private void FillMusicFilesWithEntries(List<MusicFile> musicFiles, IEnumerable<Metadata> entries, DataProvider provider)
    {
        foreach (var fileMetadata in entries)
        {
            if (fileMetadata.Tag == "folder")
            {
                continue;
            }

            var musicFile = MapEntryToMusicFile(fileMetadata);

            if (musicFile.Type == AudioTypes.Unknown)
            {
                continue;
            }

            musicFile.DataProvider = provider;
            musicFiles.Add(musicFile);
        }
    }

    private static MusicFile MapEntryToMusicFile(Metadata file)
    {
        return new MusicFile
        {
            FileId = file.Id,
            Hash = file.ContentHash,
            Name = file.Name,
            Size = file.Size,
            Path = file.PathDisplay,
            Type = GetAudioType(file)
        };
    }

    private static AudioTypes GetAudioType(Metadata file)
    {
        if (file.PathDisplay.EndsWith("flac", StringComparison.InvariantCultureIgnoreCase))
        {
            return AudioTypes.Flac;
        }

        if (file.PathDisplay.EndsWith("mp3", StringComparison.InvariantCultureIgnoreCase))
        {
            return AudioTypes.Mp3;
        }

        return AudioTypes.Unknown;
    }
}
