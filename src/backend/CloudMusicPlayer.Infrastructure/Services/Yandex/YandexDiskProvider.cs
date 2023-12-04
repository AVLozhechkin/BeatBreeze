using System.Net.Http.Json;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Services.Yandex.Results;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.Services.Yandex;

internal sealed class YandexDiskProvider(
    HttpClient _httpClient,
    IEncryptionService _encryptionService,
    IOptions<YandexOptions> options)
    : IExternalProviderService, IDisposable
{
    private const string FilesUrl = "https://cloud-api.yandex.net/v1/disk/resources/files/";
    private const string RefreshAccessTokenUrl = "https://oauth.yandex.ru/";
    private const string DownloadUrl = "https://cloud-api.yandex.net/v1/disk/resources/download";

    private const int Limit = 1000;
    private const string Fields = "items.name,items.size,items.path, " +
                                  "items.md5, items.resource_id, offset, limit";

    private readonly YandexOptions _options = options.Value;

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Yandex;
    }

    public async Task<IReadOnlyList<SongFile>> GetSongFiles(DataProvider provider)
    {
        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {token}");
        var uri = $"{FilesUrl}?media_type=audio&limit={Limit}&fields={Fields}";

        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var files = await response.Content.ReadFromJsonAsync<FilesResult>();

        if (files is null)
        {
            throw new ExternalApiException("Files response was not properly deserialized");
        }

        // TODO Check if there is a way to fetch more than LIMIT files
        var songFiles = new List<SongFile>(files.Items.Count);

        for (int i = 0; i < files.Items.Count; i++)
        {
            var sf = files.Items[i].MapToSongFile();

            if (sf.Type == AudioTypes.Unknown)
            {
                continue;
            }

            sf.DataProvider = provider;
            songFiles.Add(sf);
        }

        return songFiles;
    }

    public async Task<string> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        var token = _encryptionService.Decrypt(provider.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {token}");
        var uri = $"{DownloadUrl}/?path={songFile.Path}";

        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var link = await response.Content.ReadFromJsonAsync<TemporaryLinkResult>();

        if (link is null)
        {
            throw new ExternalApiException("TemporaryLink was not properly deserialized");
        }

        return link.Link;
    }

    public async Task<AccessToken> GetAccessToken(byte[] refreshToken)
    {
        var args = new Dictionary<string, string>()
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", _encryptionService.Decrypt(refreshToken) },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        var response = await _httpClient.PostAsync(RefreshAccessTokenUrl, new FormUrlEncodedContent(args));
        response.EnsureSuccessStatusCode();

        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        if (accessToken is null)
        {
            throw new ExternalApiException("AccessToken was not properly deserialized");
        }

        return accessToken;
    }

    public void Dispose() => _httpClient.Dispose();
}
