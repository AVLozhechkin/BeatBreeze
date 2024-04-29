using System.Net;
using BeatBreeze.Core.Enums;
using Microsoft.Extensions.Logging;

namespace BeatBreeze.Infrastructure.Services;

// Not the best solution for logging (except high perf), I just wanted to try it
internal static partial class LoggerMessageDefinitionsForServices
{
    // GetMusicFiles
    [LoggerMessage(LogLevel.Information, "Getting music files for {ProviderType} data provider ({ProviderId})")]
    internal static partial void LogInformation_GetMusicFiles_Start(this ILogger logger, ProviderTypes providerType, Guid providerId);
    [LoggerMessage(LogLevel.Information, "Getting remaining music files for {ProviderType} data provider ({ProviderId})")]
    internal static partial void LogInformation_GetMusicFiles_Remaining(this ILogger logger, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Information, "Received {MusicFilesCount} music files for {ProviderType} provider ({ProviderId})")]
    internal static partial void LogInformation_GetMusicFiles_Result(this ILogger logger, int musicFilesCount, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Error, "Requesting music files for {ProviderType} data provider ({ProviderId}) led to an error response with {StatusCode} code")]
    internal static partial void LogError_GetMusicFiles_BadResponse(this ILogger logger, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode);

    [LoggerMessage(LogLevel.Error, "Requesting music files for {ProviderType} data provider ({ProviderId}) led to successful response with {StatusCode} code, but it's body can't be deserialized: {ResponseBody}")]
    internal static partial void LogError_GetMusicFiles_CantSerialize(this ILogger logger, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode, string responseBody);

    // GetMusicFileUrl
    [LoggerMessage(LogLevel.Information, "Getting url for music file ({MusicFileId}) from {ProviderType} data provider ({ProviderId})")]
    internal static partial void LogInformation_GetMusicFileUrl_Start(this ILogger logger, Guid musicFileId, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Information, "Url for music file ({MusicFileId}) from {ProviderType} data provider ({ProviderId}) was successfully received")]
    internal static partial void LogInformation_GetMusicFileUrl_Result(this ILogger logger, Guid musicFileId, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Error, "Getting url for music file ({MusicFileId}) from {ProviderType} data provider ({ProviderId}) led to an error response with {StatusCode} code")]
    internal static partial void LogError_GetMusicFileUrl_BadResponse(this ILogger logger, Guid musicFileId, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode);

    [LoggerMessage(LogLevel.Error, "Getting url for music file ({MusicFileId}) from {ProviderType} data provider ({ProviderId}) led to successful response with {StatusCode} code, but it's body can't be deserialized: {ResponseBody}")]
    internal static partial void LogError_GetMusicFileUrl_CantSerialize(this ILogger logger, Guid musicFileId, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode, string responseBody);

    // GetAccessToken
    [LoggerMessage(LogLevel.Information, "Getting an access token for {ProviderType} data provider ({ProviderId})")]
    internal static partial void LogInformation_GetAccessToken_Start(this ILogger logger, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Information, "An access token for {ProviderType} data provider ({ProviderId}) was successfully requested")]
    internal static partial void LogInformation_GetAccessToken_Result(this ILogger logger, ProviderTypes providerType, Guid providerId);

    [LoggerMessage(LogLevel.Error, "Getting an access token for {ProviderType} data provider ({ProviderId}) led to an error response with {StatusCode} code")]
    internal static partial void LogError_GetAccessToken_BadResponse(this ILogger logger, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode);

    [LoggerMessage(LogLevel.Error, "Getting an access token for {ProviderType} data provider ({ProviderId}) led to successful response with {StatusCode} code, but it's body can't be deserialized: {ResponseBody}")]
    internal static partial void LogError_GetAccessToken_CantSerialize(this ILogger logger, ProviderTypes providerType, Guid providerId, HttpStatusCode statusCode, string responseBody);
}
