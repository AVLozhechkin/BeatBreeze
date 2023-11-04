using System.Net.Http.Json;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;
using CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox;

public sealed class DropboxProvider : IExternalProviderService
{
    private const string FilesUrl = "https://api.dropboxapi.com/2/files/list_folder";
    private const string FilesContinueUrl = "https://api.dropboxapi.com/2/files/list_folder/continue";
    private const string TemporaryLinkUrl = "https://api.dropboxapi.com/2/files/get_temporary_link";
    private const string RefreshAccessTokenUrl = "https://api.dropbox.com/oauth2/token";
    private const ushort Limit = 2000;


    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DropboxOptions _options;

    public DropboxProvider(IHttpClientFactory httpClientFactory, IOptions<DropboxOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Dropbox;
    }

    public async Task<IReadOnlyList<SongFile>> GetSongFiles(DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {provider.AccessToken.Token}");

        var defaultBody = new ListFolderArg
        {
            Limit = Limit,
            Path = "", // root of the disk
            Recursive = true
        };

        var response = await httpClient.PostAsJsonAsync(FilesUrl, defaultBody);

        var listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();

        // TODO Add handling errors

        var songFiles = new List<SongFile>(listFolderResult!.Entries.Count);

        foreach (var fileMetadata in listFolderResult.Entries)
        {
            if (fileMetadata.Tag == "folder")
            {
                continue;
            }

            var songFile = MapEntryToSongFile(fileMetadata);
            songFile.DataProvider = provider;
            songFiles.Add(songFile);
        }

        while (listFolderResult.HasMore)
        {
            var cursorBody = new ListFolderContinueArg() { Cursor = listFolderResult.Cursor};
            response = await httpClient.PostAsJsonAsync(FilesContinueUrl, cursorBody);
            listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();

            foreach (var fileMetadata in listFolderResult!.Entries)
            {
                if (fileMetadata.Tag == "folder")
                {
                    continue;
                }

                songFiles.Add(MapEntryToSongFile(fileMetadata));
            }
        }

        return songFiles;
    }

    public async Task<string?> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {provider.AccessToken.Token}");

        var body = new TemporaryLinkArg
        {
            Path = songFile.Path
        };

        var response = await httpClient.PostAsJsonAsync(TemporaryLinkUrl, body);

        var temporaryLink = await response.Content.ReadFromJsonAsync<TemporaryLinkResult>();

        return temporaryLink?.Link;
    }

    public async Task<AccessTokenResult?> GetApiToken(string refreshToken)
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
        var accessToken = await response.Content.ReadFromJsonAsync<AccessTokenResult>();

        return accessToken;

    }


    private static SongFile MapEntryToSongFile(Metadata file)
    {
        return new SongFile
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
        if (file.PathDisplay.EndsWith("flac"))
        {
            return AudioTypes.Flac;
        }

        if (file.PathDisplay.EndsWith("mp3"))
        {
            return AudioTypes.Mp3;
        }

        return AudioTypes.Unknown;
    }
}
