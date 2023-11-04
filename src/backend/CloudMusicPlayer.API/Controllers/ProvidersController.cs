using System.Security.Claims;
using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Services;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ProvidersController : ControllerBase
{
    private readonly ProviderService _providerService;

    public ProvidersController(ProviderService providerService, AuthService authService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DataProviderDto>>> GetListOfDataProviders()
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return BadRequest("Something wrong with cookies. Please re-login.");
        }

        var providersResult = await _providerService.GetAllProvidersByUserId(userIdResult.Value);

        if(providersResult.IsFailure)
        {
            // switch return
            return BadRequest(providersResult.Error);
        }

        return Ok(providersResult.Value.Select(DataProviderDto.Create));
    }

    [HttpGet("{providerId:required}")]
    public async Task<ActionResult<DataProviderDto>> GetDataProviderWithContent(Guid providerId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return BadRequest("Something wrong with cookies. Please re-login.");
        }

        var providerResult = await _providerService.GetDataProvider( providerId, userIdResult.Value);

        if (providerResult.IsFailure)
        {
            // switch return
            return BadRequest(providerResult.Error);
        }

        return Ok(DataProviderDto.Create(providerResult.Value!));
    }

    [HttpPost("{providerId:required}")]
    public async Task<ActionResult<DataProvider>> UpdateDataProviderContent(Guid providerId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return BadRequest("Something wrong with cookies. Please re-login.");
        }

        var updatedProviderResult = await _providerService.UpdateDataProvider(providerId, userIdResult.Value);

        if (updatedProviderResult.IsFailure)
        {
            // switch return
            return BadRequest(updatedProviderResult.Error);
        }

        return Ok(DataProviderDto.Create(updatedProviderResult.Value!));
    }

    [HttpDelete("{providerId:required}")]
    public async Task<IActionResult> RemoveDataProvider(Guid providerId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return BadRequest("Something wrong with cookies. Please re-login.");
        }

        var providersResult = await _providerService.RemoveDataProvider(providerId, userIdResult.Value);

        if (providersResult.IsFailure)
        {
            return BadRequest(providersResult.Error);
        }

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

        var provider = await
            _providerService
                .AddDataProvider(ProviderTypes.Yandex, userId, name, apiToken, refreshToken, expiresAt);

        if (provider.IsFailure)
        {
            // make switch
            return Redirect("/providers");
        }

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

        var provider = await
            _providerService
                .AddDataProvider(ProviderTypes.Dropbox, userId, name, apiToken, refreshToken, expiresAt);

        if (provider.IsFailure)
        {
            // make switch
            return Redirect("/providers");
        }

        return Redirect("/providers");
    }

    [HttpGet("songUrl/{songFileId:required}")]
    public async Task<ActionResult<string>> GetSongUrl(Guid songFileId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return BadRequest("Something wrong with cookies. Please re-login.");
        }

        var urlResult = await _providerService.GetSongFileUrl(songFileId, userIdResult.Value);

        if (urlResult.IsFailure)
        {
            return BadRequest(urlResult.Error);
        }

        return Ok(urlResult.Value);
    }
}
