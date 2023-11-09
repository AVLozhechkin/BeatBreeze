using System.Net.Http.Json;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.DataProviders.Yandex.Results;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex;

internal sealed class YandexDiskProvider : IExternalProviderService
{
    private const string FilesUrl = "https://cloud-api.yandex.net/v1/disk/resources/files/";
    private const string RefreshAccessTokenUrl = "https://oauth.yandex.ru/";
    private const string DownloadUrl = "https://cloud-api.yandex.net/v1/disk/resources/download";

    private const int Limit = 1000;
    private const string Fields = "items.name,items.size,items.path, " +
                                  "items.md5, items.resource_id, offset, limit";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly YandexOptions _options;

    public YandexDiskProvider(IHttpClientFactory httpClientFactory, IOptions<YandexOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Yandex;
    }

    public async Task<Result<IReadOnlyList<SongFile>>> GetSongFiles(DataProvider provider)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {provider.AccessToken}");

        var uri = $"{FilesUrl}?media_type=audio&limit={Limit}&fields={Fields}";

        var response = await httpClient.GetFromJsonAsync<FilesResult>(uri, CancellationToken.None);

        var songFiles = new SongFile[response!.Items.Count];

        for (int i = 0; i < songFiles.Length; i++)
        {
            songFiles[i] = response.Items[i].MapToSongFile(provider);
        }

        return songFiles;
    }

    public async Task<Result<string>> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {provider.AccessToken}");

        var response = await httpClient.GetFromJsonAsync<TemporaryLinkResult>($"{DownloadUrl}/?path={songFile.Path}");

        return response?.Link;
    }

    public async Task<Result<AccessToken>> GetApiToken(string refreshToken)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var args = new Dictionary<string, string>()
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        var response = await httpClient.PostAsync(RefreshAccessTokenUrl, new FormUrlEncodedContent(args));
        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        return accessToken;
    }
}
