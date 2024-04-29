using System.Security.Claims;
using BeatBreeze.API.Dtos.Models;
using BeatBreeze.API.Utils;
using BeatBreeze.Core.Enums;
using BeatBreeze.Core.Exceptions;
using BeatBreeze.Core.Interfaces;
using BeatBreeze.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BeatBreeze.API.Controllers;

[Authorize]
public sealed class ProvidersController : BaseController
{
    private readonly IProviderService _providerService;
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public ProvidersController(IProviderService providerService, IDistributedCache cache)
    {
        _providerService = providerService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IEnumerable<DataProviderDto>> GetDataProvidersByUserId(bool includeFiles = false)
    {
        var userId = User.GetUserGuid();

        var providers = await _providerService.GetProvidersByUserId(userId, includeFiles);

        return providers.Select(dp => DataProviderDto.Create(dp, includeFiles));
    }

    [HttpGet("{providerId:required}")]
    public async Task<ActionResult<DataProviderDto>> GetById(Guid providerId, bool includeFiles = false)
    {
        var userId = User.GetUserGuid();

        var provider = await _providerService.GetDataProviderById(providerId, userId, includeFiles);

        if (provider is null)
        {
            throw NotFoundException.Create<DataProvider>(providerId);
        }

        return Ok(DataProviderDto.Create(provider, includeFiles));
    }

    [HttpPatch("{providerId:required}")]
    public async Task<DataProviderDto> UpdateDataProviderContent(Guid providerId, bool includeFiles)
    {
        var userId = User.GetUserGuid();

        var provider = await _providerService.UpdateDataProvider(providerId, userId);

        return DataProviderDto.Create(provider, includeFiles);
    }

    [HttpDelete("{providerId:required}")]
    public async Task RemoveDataProvider(Guid providerId)
    {
        var userId = User.GetUserGuid();

        await _providerService.RemoveDataProvider(providerId, userId);
    }

    [HttpGet("add-provider/{providerType:required}")]
    public IActionResult AddYandex(ProviderTypes providerType)
    {
        AuthenticationProperties? properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("OAuthCallback", new { providerType }),
            Items = { ["LoginProvider"] = providerType.ToString() }
        };

        return Challenge(properties, properties.Items["LoginProvider"]!);
    }

    [HttpGet("callback/{providerType}")]
    public async Task<IActionResult> OAuthCallback(ProviderTypes providerType)
    {
        var authSchema = providerType switch
        {
            ProviderTypes.Yandex => "Yandex",
            ProviderTypes.Dropbox => "Dropbox",
            ProviderTypes.Google => "Google",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(authSchema))
        {
            return Redirect("/providers?errors=unknown_schema");
        }

        var authenticateResult = await HttpContext.AuthenticateAsync(authSchema);
        if (!authenticateResult.Succeeded)
        {
            return Redirect("/providers?errors=oauth_failed");
        }

        var name = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value!;
        var apiToken = authenticateResult.Properties?.GetTokenValue("access_token")!;
        var refreshToken = authenticateResult.Properties?.GetTokenValue("refresh_token")!;
        var expiresAt = authenticateResult.Properties?.GetTokenValue("expires_at")!;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest("UserId must be a parseable GUID");
        }

        await _providerService
                .AddDataProvider(providerType, userId, name, apiToken, refreshToken, expiresAt);

        HttpContext.Response.Cookies.Delete("BeatBreeze_External");

        return Redirect("/providers");
    }

    [HttpGet("songUrl/{musicFileId:required}")]
    public async Task<ActionResult<string>> GetSongUrl(Guid musicFileId, bool notCached = false)
    {
        var userId = User.GetUserGuid();

        var cacheKey = $"{musicFileId}_{userId}";

        if (notCached)
        {
            var url = await _providerService.GetMusicFileUrl(musicFileId, userId);

            await _cache.SetStringAsync(cacheKey, url, cacheOptions);

            return Ok(url);
        }

        var cachedUrl = await _cache.GetStringAsync(cacheKey);

        if (cachedUrl is null)
        {
            var url = await _providerService.GetMusicFileUrl(musicFileId, userId);

            await _cache.SetStringAsync(cacheKey, url, cacheOptions);

            return Ok(url);
        }

        return Ok(cachedUrl);
    }
}
