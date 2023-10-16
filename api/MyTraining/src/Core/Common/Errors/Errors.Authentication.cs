namespace Core.Common.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials => Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Invalid credentials.");
        public static Error InvalidLogin => Error.Unauthorized(code: "Auth.InvalidLogin", description: "Invalid login.");
        public static Error InvalidPassword => Error.Unauthorized(code: "Auth.InvalidPassword", description: "Invalid password.");
        public static Error InvalidToken => Error.Unauthorized(code: "Auth.InvalidToken", description: "Invalid token.");
    }
}