using System.Text.Json;
using System.Web;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Services.Yandex.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudMusicPlayer.Core.Exceptions;

namespace CloudMusicPlayer.Infrastructure.Services.Yandex;

internal sealed class YandexDiskProvider : IExternalProviderService
{
    private const string FilesPath = "files";
    private const string DownloadPath = "download";

    private const int Limit = 1000;
    private const string Fields = "items.name,items.size,items.path, " +
                                  "items.md5, items.resource_id, offset, limit";

    public YandexDiskProvider(HttpClient httpClient,
        IEncryptionService encryptionService,
        ILogger<YandexDiskProvider> logger,
        IOptions<YandexOptions> options)
    {
        _httpClient = httpClient;
        _encryptionService = encryptionService;
        _logger = logger;
        _options = options.Value;
    }

    private readonly YandexOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<YandexDiskProvider> _logger;

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Yandex;
    }

    public async Task<IReadOnlyList<MusicFile>> GetMusicFiles(DataProvider provider)
    {
        _logger.LogInformation_GetMusicFiles_Start(ProviderTypes.Yandex, provider.Id);

        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {token}");

        var uri = $"{Path.Combine(_options.ResourceUrl, FilesPath)}?media_type=audio&limit={Limit}&fields={Fields}";

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetMusicFiles_BadResponse(ProviderTypes.Yandex, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "Yandex is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var files = JsonSerializer.Deserialize<FilesResult>(body);

        if (files is null)
        {
            _logger.LogError_GetMusicFiles_CantSerialize(ProviderTypes.Yandex, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "Files response was not properly deserialized.");
        }

        // TODO Check if there is a way to fetch more than LIMIT files
        var musicFiles = new List<MusicFile>(files.Items.Count);

        for (int i = 0; i < files.Items.Count; i++)
        {
            var sf = files.Items[i].MapToMusicFile();

            if (sf.Type == AudioTypes.Unknown)
            {
                continue;
            }

            sf.DataProvider = provider;
            musicFiles.Add(sf);
        }

        _logger.LogInformation_GetMusicFiles_Result(musicFiles.Count, ProviderTypes.Yandex, provider.Id);
        return musicFiles;
    }

    public async Task<string> GetMusicFileUrl(MusicFile musicFile, DataProvider provider)
    {
        _logger.LogInformation_GetMusicFileUrl_Start(musicFile.Id, ProviderTypes.Yandex, provider.Id);

        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {token}");

        var encodedPath = HttpUtility.UrlEncode(musicFile.Path);

        var uri = $"{Path.Combine(_options.ResourceUrl, DownloadPath)}/?path={encodedPath}";

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetMusicFileUrl_BadResponse(musicFile.Id, ProviderTypes.Yandex, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "Yandex is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var link = JsonSerializer.Deserialize<TemporaryLinkResult>(body); 

        if (link is null)
        {
            _logger.LogError_GetMusicFileUrl_CantSerialize(musicFile.Id, ProviderTypes.Yandex, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "TemporaryLink was not properly deserialized");
        }

        _logger.LogInformation_GetMusicFileUrl_Result(musicFile.Id, ProviderTypes.Yandex, provider.Id);

        return link.Link;
    }

    public async Task<AccessToken> GetAccessToken(DataProvider provider)
    {
        _logger.LogInformation_GetAccessToken_Start(ProviderTypes.Yandex, provider.Id);

        var args = new Dictionary<string, string>()
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", _encryptionService.Decrypt(provider.RefreshToken) },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        using var content = new FormUrlEncodedContent(args);

        var response = await _httpClient.PostAsync(_options.OAuthUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError_GetAccessToken_BadResponse(ProviderTypes.Yandex, provider.Id, response.StatusCode);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "Yandex is unavailable. Please, come back later.");
        }

        var body = await response.Content.ReadAsStringAsync();

        var accessToken = JsonSerializer.Deserialize<AccessToken>(body);

        if (accessToken is null)
        {
            _logger.LogError_GetAccessToken_CantSerialize(ProviderTypes.Yandex, provider.Id, response.StatusCode, body);
            throw ExternalApiException.Create(ProviderTypes.Yandex, response.StatusCode, "Yandex is unavailable. Please, come back later.");
        }

        _logger.LogInformation_GetAccessToken_Result(ProviderTypes.Yandex, provider.Id);

        return accessToken;
    }
}
