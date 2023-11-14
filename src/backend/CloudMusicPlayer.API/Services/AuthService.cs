using System.Security.Claims;
using System.Text.RegularExpressions;
using CloudMusicPlayer.API.Errors;
using CloudMusicPlayer.Core;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CloudMusicPlayer.API.Services;

public sealed class AuthService
{
    private readonly IPasswordHasher<User> _hasher;
    private readonly UserService _userService;
    private const string PasswordPattern = "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[\\]:;<>,.?/~_+-=|\\\\]).{8,32}$";

    public AuthService(IPasswordHasher<User> hasher, UserService userService)
    {
        _hasher = hasher;
        _userService = userService;
    }

    public async Task<Result<User>> CreateUser(HttpContext httpContext, string email, string name, string password)
    {
        var passwordHashResult = HashPassword(password);

        if (passwordHashResult.IsFailure)
        {
            return Result.Failure<User>(passwordHashResult.Error);
        }

        var userCreationResult = await _userService.CreateUser(email, name, passwordHashResult.Value);

        if (userCreationResult.IsFailure)
        {
            return Result.Failure<User>(userCreationResult.Error);
        }

        await SignInWithCookies(httpContext, userCreationResult.Value);

        return userCreationResult;
    }

    public async Task<Result<User>> SignIn(HttpContext httpContext, string email, string password)
    {
        var userSearchResult = await _userService.GetUserByEmail(email);

        if (userSearchResult.IsFailure)
        {
            return userSearchResult;
        }

        if (userSearchResult.Value is null)
        {
            return Result.Failure<User>(DomainLayerErrors.NotFound());
        }

        var verificationResult = _hasher.VerifyHashedPassword(null!, userSearchResult.Value.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Result.Failure<User>(ApplicationLayerErrors.Auth.PasswordVerificationFailure());
        }

        await SignInWithCookies(httpContext, userSearchResult.Value);

        return Result.Success(userSearchResult.Value);
    }

    private async Task SignInWithCookies(HttpContext ctx, User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await ctx.SignInAsync(principal);
    }

    private Result<string> HashPassword(string password)
    {
        var validationResult = ValidatePasswordRequirements(password);

        if (validationResult.IsFailure)
        {
            return Result.Failure<string>(validationResult.Error);
        }

        var passwordHash = _hasher.HashPassword(null!, password);

        return Result.Success(passwordHash);
    }

    private static Result ValidatePasswordRequirements(string password)
    {
        if (Regex.IsMatch(password, PasswordPattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
        {
            return Result.Success();
        }

        return Result.Failure(ApplicationLayerErrors.Auth.PasswordDoesNotMeetRequirements());
    }
}
