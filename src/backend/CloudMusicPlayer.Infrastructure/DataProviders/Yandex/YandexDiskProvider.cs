using System.Net.Http.Json;
using CloudMusicPlayer.Core;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.DataProviders.Yandex.Results;
using CloudMusicPlayer.Infrastructure.Errors;
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

        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(uri);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.HttpError());
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
        }

        var files = await response.Content.ReadFromJsonAsync<FilesResult>();

        if (files is null)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnknownResponse());
        }

        // TODO Check if there is a way to fetch more than LIMIT files
        var songFiles = new List<SongFile>(files.Items.Count);

        for (int i = 0; i < files.Items.Count(); i++)
        {
            songFiles[i] = files.Items[i].MapToSongFile(provider);
        }

        return Result.Success<IReadOnlyList<SongFile>>(songFiles);
    }

    public async Task<Result<string>> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {provider.AccessToken}");
        var uri = $"{DownloadUrl}/?path={songFile.Path}";

        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(uri);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(DataLayerErrors.Http.HttpError());
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<string>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
        }

        var link = await response.Content.ReadFromJsonAsync<TemporaryLinkResult>();

        if (link is null)
        {
            return Result.Failure<string>(DataLayerErrors.Http.UnknownResponse());
        }

        return Result.Success(link.Link);
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

        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsync(RefreshAccessTokenUrl, new FormUrlEncodedContent(args));
        }
        catch (Exception ex)
        {
            return Result.Failure<AccessToken>(DataLayerErrors.Http.HttpError());
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<AccessToken>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
        }

        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        if (accessToken is null)
        {
            return Result.Failure<AccessToken>(DataLayerErrors.Http.UnknownResponse());
        }

        return Result<AccessToken>.Success(accessToken);
    }
}
