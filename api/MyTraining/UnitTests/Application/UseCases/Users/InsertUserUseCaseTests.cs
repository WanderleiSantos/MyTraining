using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using MyTraining.Application.UseCases.Users.InsertUser;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;
using MyTraining.Application.UseCases.Users.InsertUser.Validations;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace UnitTests.Application.UseCases.Users;

public class InsertUserUseCaseTests
{
    private readonly Mock<ILogger<InsertUserUseCase>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly IValidator<InsertUserCommand> _validator;
    private readonly IInsertUserUseCase _useCase;

    private readonly Faker _faker;

    public InsertUserUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<InsertUserUseCase>>();
        _repositoryMock = new Mock<IUserRepository>();
        _validator = new InsertUserCommandValidator();

        _useCase = new InsertUserUseCase(
            _loggerMock.Object,
            _repositoryMock.Object,
            _validator
        );

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandIsEmpty()
    {
        //Given
        var command = new InsertUserCommand();
        var cancelationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancelationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(8);
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'First Name' must not be empty.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("The length of 'First Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Last Name' must not be empty.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("The length of 'Last Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Email' must not be empty.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Email' is not a valid email address.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("Password must contain at least 8 characters, one number, one uppercase letter, one lowercase letter and one special character")).Which.Code.Should().Be("Password");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Password' must not be empty.")).Which.Code.Should().Be("Password");

    }

    [Fact]
    public async Task ShouldReturnRrrorValidationIfTheEmailIsInvalid()
    {
        var command = new InsertUserCommand() { FirstName = _faker.Random.String2(10), LastName = _faker.Random.String2(10), Password = _faker.Internet.Password(10), Email = "e-mail.com.br" };
        var cancelationToken = CancellationToken.None;

        var output = await _useCase.ExecuteAsync(command, cancelationToken);

        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Email' is not a valid email address.")).Which.Code.Should().Be("Email");
    }


    private InsertUserCommand CreateCommand() => new InsertUserCommand
    {
        Email = _faker.Internet.Email(),
        Password = _faker.Random.String2(10),
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };
}