using System.Net.Http.Json;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;
using CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox;

internal sealed class DropboxProvider : IExternalProviderService
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

    public async Task<Result<string>> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {provider.AccessToken}");

        var body = new TemporaryLinkArg
        {
            Path = songFile.Path
        };

        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsJsonAsync(TemporaryLinkUrl, body);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(DataLayerErrors.Http.HttpError());
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<string>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
        }

        var temporaryLink = await response.Content.ReadFromJsonAsync<TemporaryLinkResult>();

        if (temporaryLink is null)
        {
            return Result.Failure<string>(DataLayerErrors.Http.UnknownResponse());
        }

        return Result.Success(temporaryLink.Link);
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

        try
        {
            var response = await httpClient.PostAsync(RefreshAccessTokenUrl, new FormUrlEncodedContent(args));
            var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

            if (accessToken is null)
            {
                return Result.Failure<AccessToken>(DataLayerErrors.Http.UnknownResponse());
            }

            return Result.Success(accessToken);
        }
        catch (Exception ex)
        {
            return Result.Failure<AccessToken>(DataLayerErrors.Http.HttpError());
        }
    }

    public async Task<Result<IReadOnlyList<SongFile>>> GetSongFiles(DataProvider provider)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {provider.AccessToken}");

        var defaultBody = new ListFolderArg
        {
            Limit = Limit,
            Path = "", // root of the disk
            Recursive = true
        };

        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsJsonAsync(FilesUrl, defaultBody);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.HttpError());
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
        }

        var listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();

        if (listFolderResult is null)
        {
            return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnknownResponse());
        }

        var songFiles = new List<SongFile>(listFolderResult.Entries.Count);

        FillSongFiles(songFiles, listFolderResult.Entries, provider);

        while (listFolderResult.HasMore)
        {
            var cursorBody = new ListFolderContinueArg() { Cursor = listFolderResult.Cursor};

            try
            {
                response = await httpClient.PostAsJsonAsync(FilesContinueUrl, cursorBody);
            }
            catch (Exception ex)
            {
                return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.HttpError());
            }

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnsuccessfulStatusCode(response.StatusCode));
            }

            listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();

            if (listFolderResult is null)
            {
                return Result.Failure<IReadOnlyList<SongFile>>(DataLayerErrors.Http.UnknownResponse());
            }

            FillSongFiles(songFiles, listFolderResult.Entries, provider);
        }

        return Result.Success<IReadOnlyList<SongFile>>(songFiles);
    }

    private void FillSongFiles(List<SongFile> songFiles, IList<Metadata> entries, DataProvider provider)
    {
        foreach (var fileMetadata in entries)
        {
            if (fileMetadata.Tag == "folder")
            {
                continue;
            }

            var songFile = MapEntryToSongFile(fileMetadata);
            songFile.DataProvider = provider;
            songFiles.Add(songFile);
        }
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
