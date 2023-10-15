using FluentValidation.Results;

namespace Application.Shared.Models;

public class Output
{
    private readonly List<Notification> _errorMessages = new();

    public EErrorType? ErrorType { get; set; }
    public object? Result { get; private set; }
    public bool IsValid => !_errorMessages.Any();
    public IReadOnlyCollection<Notification> ErrorMessages => _errorMessages;

    public void AddErrorMessage(string code, string description) => _errorMessages.Add(new Notification(code, description));
    public void AddErrorMessage(string description) => _errorMessages.Add(new Notification(null, description));
    public void AddValidationResult(ValidationResult validationResult)
    {
        _errorMessages.AddRange(validationResult.Errors.Select(e => new Notification(e.PropertyName, e.ErrorMessage))
            .ToList());
        ErrorType = _errorMessages.Any() ? Models.EErrorType.Validation : null;
    }
    
    public void AddResult(object? result) => Result = result;
}

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
    
    public static Output SetErrorType(this Output output, EErrorType? errorType)
    {
        output.ErrorType = errorType;
        return output;
    }
}