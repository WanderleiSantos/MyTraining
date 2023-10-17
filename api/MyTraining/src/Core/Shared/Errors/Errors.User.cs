namespace Core.Shared.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail => Error.Conflict(code: "User.DuplicateEmail", description: "Email is already taken.");
        public static Error DoesNotExist => Error.NotFound(code: "User.DoesNotExist", description: "User does not exist.");
        public static Error InvalidPassword => Error.Validation(code: "User.InvalidPassword", description: "Invalid password.");
    }
}