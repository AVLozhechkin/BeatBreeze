using System.Net.Http.Json;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Services.Dropbox.Args;
using CloudMusicPlayer.Infrastructure.Services.Dropbox.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Infrastructure.Services.Dropbox;

internal sealed class DropboxProvider(
    HttpClient httpClient,
    IEncryptionService encryptionService,
    ILogger<DropboxProvider> logger,
    IOptions<DropboxOptions> options)
    : IExternalProviderService, IDisposable
{
    private const string FilesUrl = "https://api.dropboxapi.com/2/files/list_folder";
    private const string FilesContinueUrl = "https://api.dropboxapi.com/2/files/list_folder/continue";
    private const string TemporaryLinkUrl = "https://api.dropboxapi.com/2/files/get_temporary_link";
    private const string RefreshAccessTokenUrl = "https://api.dropbox.com/oauth2/token";
    private const ushort Limit = 2000;


    private readonly DropboxOptions _options = options.Value;

    public bool CanBeExecuted(ProviderTypes providerType)
    {
        return providerType == ProviderTypes.Dropbox;
    }

    public async Task<string> GetSongFileUrl(SongFile songFile, DataProvider provider)
    {
        logger.LogInformation("Requesting URL for songFile with ID: {SongFileId}", songFile.Id.ToString());

        var token = encryptionService.Decrypt(provider.AccessToken);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var body = new TemporaryLinkArg
        {
            Path = songFile.Path
        };

        var response = await httpClient.PostAsJsonAsync(TemporaryLinkUrl, body);
        response.EnsureSuccessStatusCode();

        var temporaryLink = await response.Content.ReadFromJsonAsync<TemporaryLinkResult>();

        if (temporaryLink is null)
        {
            logger.LogWarning("Status code is success ({StatusCode}), but request deserialization is failed", response.StatusCode);
            throw new ExternalApiException("Link response was not properly deserialized");
        }

        logger.LogInformation("URL for songFile with ID: {SongFileId} was successfully received", songFile.Id.ToString());
        return temporaryLink.Link;
    }

    public async Task<AccessToken> GetAccessToken(byte[] refreshToken)
    {
        logger.LogInformation("Requesting new access token");

        var decryptedRefreshToken = encryptionService.Decrypt(refreshToken);

        var args = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", decryptedRefreshToken },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
        };

        var response = await httpClient.PostAsync(RefreshAccessTokenUrl, new FormUrlEncodedContent(args));
        response.EnsureSuccessStatusCode();

        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        if (accessToken is null)
        {
            logger.LogWarning("Status code is success ({StatusCode}), but request deserialization is failed", response.StatusCode);
            throw new ExternalApiException("AccessToken response was not properly deserialized");
        }

        logger.LogInformation("An access token was successfully requested");

        return accessToken;
    }

    public async Task<IReadOnlyList<SongFile>> GetSongFiles(DataProvider provider)
    {
        var userId = provider.User?.Id ?? provider.UserId;

        var token = encryptionService.Decrypt(provider.AccessToken);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var defaultBody = new ListFolderArg
        {
            Limit = Limit,
            Path = "", // root of the disk
            Recursive = true
        };

        var response = await httpClient.PostAsJsonAsync(FilesUrl, defaultBody);
        response.EnsureSuccessStatusCode();

        var listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();

        if (listFolderResult is null)
        {
            throw new ExternalApiException("ListFolder response was not properly deserialized");
        }

        var songFiles = new List<SongFile>(listFolderResult.Entries.Count);

        FillSongFilesWithEntries(songFiles, listFolderResult.Entries, provider);

        while (listFolderResult.HasMore)
        {
            var cursorBody = new ListFolderContinueArg() { Cursor = listFolderResult.Cursor};

            response = await httpClient.PostAsJsonAsync(FilesContinueUrl, cursorBody);

            response.EnsureSuccessStatusCode();

            listFolderResult = await response.Content.ReadFromJsonAsync<ListFolderResult>();
            if (listFolderResult is null)
            {
                throw new ExternalApiException("ListFolder response was not properly deserialized");
            }

            FillSongFilesWithEntries(songFiles, listFolderResult.Entries, provider);
        }

        return songFiles;
    }

    private void FillSongFilesWithEntries(IList<SongFile> songFiles, IEnumerable<Metadata> entries, DataProvider provider)
    {
        foreach (var fileMetadata in entries)
        {
            if (fileMetadata.Tag == "folder")
            {
                continue;
            }

            var songFile = MapEntryToSongFile(fileMetadata);

            if (songFile.Type == AudioTypes.Unknown)
            {
                continue;
            }

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

    public void Dispose() => httpClient.Dispose();
}
