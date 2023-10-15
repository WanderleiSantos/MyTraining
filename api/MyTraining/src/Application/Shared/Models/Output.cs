using FluentValidation.Results;

namespace Application.Shared.Models;

public class Output
{
    private readonly List<Notification> _errorMessages = new();

    public ErrorType? ErrorType { get; set; }
    public object? Result { get; private set; }
    public bool IsValid => !_errorMessages.Any();
    public IReadOnlyCollection<Notification> ErrorMessages => _errorMessages;

    public void AddErrorMessage(string code, string description) => _errorMessages.Add(new Notification(code, description));
    public void AddErrorMessage(string description) => _errorMessages.Add(new Notification(null, description));
    public void AddValidationResult(ValidationResult validationResult)
    {
        _errorMessages.AddRange(validationResult.Errors.Select(e => new Notification(e.PropertyName, e.ErrorMessage))
            .ToList());
        ErrorType = _errorMessages.Any() ? Models.ErrorType.Validation : null;
    }
    
    public void AddResult(object? result) => Result = result;
}