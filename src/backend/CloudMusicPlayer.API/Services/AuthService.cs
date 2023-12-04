using System.Security.Authentication;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.API.Services;

public sealed class AuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IUserService _userService;
    private const string PasswordPattern = "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[\\]:;<>,.?/~_+-=|\\\\]).{8,32}$";

    public AuthService(ILogger<AuthService> logger, IPasswordHasher<User> hasher, IUserService userService)
    {
        _logger = logger;
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
        if (!Regex.IsMatch(password, PasswordPattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
        {
            throw new ArgumentException(nameof(password),
                "Password must contain at least one digit, one lowercase character, one uppercase character, "
                + "one special character, at least 8 characters in length, but no more than 32");
        }
    }
}
