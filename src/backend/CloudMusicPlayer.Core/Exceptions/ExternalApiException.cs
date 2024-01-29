using System.Net;
using CloudMusicPlayer.Core.Enums;

namespace CloudMusicPlayer.Core.Exceptions;

public sealed class ExternalApiException : DomainLayerException
{
    private ExternalApiException(string apiType, int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
        ApiType = apiType;
    }

    public int StatusCode { get; set; }
    public string ApiType { get; set; }

    public static ExternalApiException Create(ProviderTypes providerType, HttpStatusCode statusCode, string message)
    {
        var apiType = providerType.ToString();

        return new ExternalApiException(apiType, (int)statusCode, message);
    }
}
