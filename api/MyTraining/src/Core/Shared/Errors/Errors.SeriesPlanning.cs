namespace Core.Shared.Errors;

public static partial class Errors
{
    public static class SeriesPlanning
    {
        public static Error DoesNotExist => Error.NotFound(code: "Planning.DoesNotExist", description: "Planning does not exist.");
    }
}