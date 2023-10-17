namespace Core.Shared.Errors;

public static partial class Errors
{
    public static class Exercise
    {
        public static Error DoesNotExist => Error.NotFound(code: "Exercise.DoesNotExist", description: "User does not exist.");
    }
}