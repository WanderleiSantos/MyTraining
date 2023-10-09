using FluentValidation.Results;

namespace Application.Shared.Models;

public class Output
{
    private readonly List<Notification> _errorMessages = new();
    private readonly List<Notification> _messages = new();

    public object? Result { get; private set; }
    public bool IsValid => !_errorMessages.Any();
    public bool HasMessages => _messages.Any();
    public IReadOnlyCollection<Notification> ErrorMessages => _errorMessages;
    public IReadOnlyCollection<Notification> Messages => _messages;

    public void AddErrorMessage(string code, string message) => _errorMessages.Add(new Notification(code, message));
    public void AddErrorMessage(string message) => _errorMessages.Add(new Notification(null, message));
    public void AddMessage(string code, string message) => _messages.Add(new Notification(code, message));
    public void AddMessage(string message) => _messages.Add(new Notification(null, message));

    public void AddValidationResult(ValidationResult validationResult) =>
        _errorMessages.AddRange(validationResult.Errors.Select(e => new Notification(e.PropertyName, e.ErrorMessage))
            .ToList());
    
    public void AddResult(object? result) => Result = result;
}