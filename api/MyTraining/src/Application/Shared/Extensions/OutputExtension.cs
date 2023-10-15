using Application.Shared.Models;

namespace Application.Shared.Extensions;

public static class OutputExtension
{
    public static Output AddError(this Output output, string code, string description)
    {
        output.AddErrorMessage(code, description);
        return output;
    }
    
    public static Output AddError(this Output output, string description)
    {
        output.AddErrorMessage(description);
        return output;
    }
    
    public static Output SetErrorType(this Output output, ErrorType? errorType)
    {
        output.ErrorType = errorType;
        return output;
    }
}