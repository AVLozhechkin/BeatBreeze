using CloudMusicPlayer.Core.Errors;

namespace CloudMusicPlayer.API.Errors;

public static class ApplicationLayerErrors
{
    public static class HttpContext
    {
        public static Error InvalidUserId() => new Error("context.userId", "Invalid userId. Please re-login.");
    }

    public static class Auth
    {
        public static Error PasswordVerificationFailure() => new Error("auth.password.verification", "Wrong password.");
        public static Error PasswordDoesNotMeetRequirements() => new Error("auth.password.requirements",
            "Password must contain at least one digit, one lowercase character, one uppercase character, " +
            "one special character, at least 8 characters in length, but no more than 32");
    }
}
