using Core.Common.Errors;
using FluentValidation.Results;

namespace Application.Shared.Models;

public class Output
{
    private readonly List<Error> _errors = new();
    
    public object? Result { get; private set; }
    public bool IsValid => !_errors.Any();
    public IReadOnlyCollection<ErrorOutput> Errors => _errors.Select(e => new ErrorOutput(e.Code, e.Description)).ToList(); 

    public ErrorType? FirstError => _errors.FirstOrDefault().Type;

    public void AddError(string code, string description, ErrorType type) => _errors.Add(Error.Custom(type, code, description));
    public void AddError(Error error) => _errors.Add(error);
    public void AddValidationResult(ValidationResult validationResult)
    {
        _errors.AddRange(validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList());
    }
    
    public void AddResult(object? result) => Result = result;
}