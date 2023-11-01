using System.Net.Http.Json;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex;

public class YandexDiskProvider : IExternalProviderService
{
    private const string FilesUrl = "https://cloud-api.yandex.net/v1/disk/resources/files/";
    private const int Limit = 1000;
    private const string Fields = "items.name,items.size,items.mime_type,items.file,items.path, " +
                                  "items.md5, items.resource_id, offset, limit";
    private readonly IHttpClientFactory _httpClientFactory;

    public YandexDiskProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Yandex;
    }

    public async Task<SongFile[]> GetSongFiles(DataProvider provider)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {provider.ApiToken}");

        var uri = $"{FilesUrl}?media_type=audio&limit={Limit}&fields={Fields}";

        var response = await httpClient.GetFromJsonAsync<FilesResponse>(uri, CancellationToken.None);

        // TODO add Polly error handling

        var songFiles = new SongFile[response!.Items.Count];

        for (int i = 0; i < songFiles.Length; i++)
        {
            songFiles[i] = response.Items[i].MapToSongFile(provider);
        }

        return songFiles;
    }
}
