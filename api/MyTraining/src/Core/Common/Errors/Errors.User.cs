namespace Core.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail => Error.Conflict(code: "User.DuplicateEmail", description: "Email is already taken.");
        public static Error DoesNotExist => Error.NotFound(code: "User.DoesNotExist", description: "User does not exist.");
        public static Error Inactive => Error.Unauthorized(code: "User.Inactive", description: "User inactive.");
        public static Error DoesNotExistOrInactive => Error.Unauthorized(code: "User.DoesNotExistOrInactive", description: "User does not exist or inactive.");
    }
}