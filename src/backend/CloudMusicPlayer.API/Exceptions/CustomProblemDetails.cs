using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Exceptions;

public class CustomProblemDetails : ProblemDetails
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-6)]
    public string? ErrorCode { get; set; }
}
