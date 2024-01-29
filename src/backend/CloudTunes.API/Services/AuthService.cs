using System.Security.Authentication;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CloudTunes.API.Dtos.Requests;
using CloudTunes.Core.Interfaces;
using CloudTunes.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

namespace CloudTunes.API.Services;

public sealed class AuthService
{
    private readonly IPasswordHasher<User> _hasher;
    private readonly IUserService _userService;

    public AuthService(IPasswordHasher<User> hasher, IUserService userService)
    {
        _hasher = hasher;
        _userService = userService;
    }

    public async Task<User> CreateUser(HttpContext httpContext, string email, string password)
    {
        var passwordHash = HashPassword(password);

        var user = await _userService.CreateUser(email, passwordHash);

        await SignInWithToken(httpContext, user);

        return user;
    }

    public async Task<User> SignIn(HttpContext httpContext, string email, string password)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user is null)
        {
            throw new AuthenticationException("User for authentication was not found");
        }

        var verificationResult = _hasher.VerifyHashedPassword(null!, user.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Password validation failed");
        }

        await SignInWithToken(httpContext, user);

        return user;
    }

    private async Task SignInWithToken(HttpContext ctx, User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await ctx.SignInAsync(principal);
    }

    private string HashPassword(string password)
    {
        ValidatePasswordRequirements(password);
        return _hasher.HashPassword(null!, password);
    }

    private static void ValidatePasswordRequirements(string password)
    {
        if (!Regex.IsMatch(password, RegexPatterns.PasswordPattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
        {
            throw new ArgumentException("Password must contain at least one digit, one lowercase character, one uppercase character, "
                + "one special character, at least 8 characters in length, but no more than 32", nameof(password));
        }
    }
}
