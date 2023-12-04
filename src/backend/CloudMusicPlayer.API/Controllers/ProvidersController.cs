using System.Security.Claims;
using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[Authorize]
public sealed class ProvidersController : BaseController
{
    private readonly IProviderService _providerService;

    public ProvidersController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DataProviderDto>>> GetListOfDataProviders()
    {
        var userId = this.User.GetUserGuid();

        var providers = await _providerService.GetAllProvidersByUserId(userId);

        return Ok(providers.Select(DataProviderDto.Create));
    }

    [HttpGet("{providerId:required}")]
    public async Task<ActionResult<DataProviderDto>> GetDataProviderWithContent(Guid providerId)
    {
        var userId = this.User.GetUserGuid();

        var provider = await _providerService.GetDataProvider( providerId, userId);

        if (provider is null)
        {
            return NotFound($"Data provider with ID: {providerId} was not found");
        }

        return Ok(DataProviderDto.Create(provider));
    }

    [HttpPost("{providerId:required}")]
    public async Task<ActionResult<DataProvider>> UpdateDataProviderContent(Guid providerId)
    {
        var userId = this.User.GetUserGuid();

        var updatedProvider = await _providerService.UpdateDataProvider(providerId, userId);

        return Ok(DataProviderDto.Create(updatedProvider));
    }

    [HttpDelete("{providerId:required}")]
    public async Task<IActionResult> RemoveDataProvider(Guid providerId)
    {
        var userId = this.User.GetUserGuid();

        await _providerService.RemoveDataProvider(providerId, userId);

        return Ok();
    }

    [HttpGet("add-provider/{providerType:required}")]
    public IActionResult AddYandex(ProviderTypes providerType)
    {
        AuthenticationProperties? properties = null;

        switch (providerType)
        {
            case ProviderTypes.Dropbox:
                properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("DropboxCallback"),
                    Items = { ["LoginProvider"] = "Dropbox" }
                };
                break;
            case ProviderTypes.Yandex:
                properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("YandexCallback"),
                    Items = { ["LoginProvider"] = "Yandex" }
                };
                break;
        }

        if (properties is null)
        {
            return BadRequest("Cant parse provider type");
        }

        return Challenge(properties, properties.Items["LoginProvider"]!);
    }

    [HttpGet("yandex-callback")]
    public async Task<IActionResult> YandexCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Yandex");
        if (!authenticateResult.Succeeded)
        {
            return BadRequest(authenticateResult.Failure?.Message);
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
                .AddDataProvider(ProviderTypes.Yandex, userId, name, apiToken, refreshToken, expiresAt);

        HttpContext.Response.Cookies.Delete("CloudMusicPlayer_External");

        return Redirect("/providers");
    }

    [HttpGet("dropbox-callback")]
    public async Task<IActionResult> DropBoxCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Dropbox");
        if (!authenticateResult.Succeeded)
        {
            return BadRequest(authenticateResult.Failure?.Message);
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
                .AddDataProvider(ProviderTypes.Dropbox, userId, name, apiToken, refreshToken, expiresAt);

        HttpContext.Response.Cookies.Delete("CloudMusicPlayer_External");

        return Redirect("/providers");
    }

    [HttpGet("songUrl/{songFileId:required}")]
    public async Task<ActionResult<string>> GetSongUrl(Guid songFileId)
    {
        var userId = this.User.GetUserGuid();

        var url = await _providerService.GetSongFileUrl(songFileId, userId);

        return Ok(url);
    }
}
