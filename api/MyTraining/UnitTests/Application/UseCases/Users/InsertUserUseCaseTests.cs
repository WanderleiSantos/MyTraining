using System.Globalization;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.InsertUser.Commands;
using Application.UseCases.Users.InsertUser.Responses;
using Application.UseCases.Users.InsertUser.Validations;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using UnitTests.Application.UseCases.Users.Shared.Extensions;

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
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
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
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(8);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'First Name' must not be empty.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'First Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Last Name' must not be empty.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'Last Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Email' must not be empty.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Email' is not a valid email address.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("Password must contain at least 8 characters, one number, one uppercase letter, one lowercase letter and one special character")).Which.Code.Should().Be("Password");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Password' must not be empty.")).Which.Code.Should().Be("Password");
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfTheEmailIsInvalid()
    {
        const string email = "e-mail.com.br";
        var command = CreateCommand(email);
        var cancellationToken = CancellationToken.None;

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Email' is not a valid email address.")).Which.Code.Should().Be("Email");
    }
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfTheEmailAlreadyExists()
    {
        const string email = "e-mail@email.com";
        var command = CreateCommand(email);
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.ExistsEmailRegisteredAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("E-mail already registered")).Which.Code.Should().Be("Email");
    }
    
    [Fact]
    public async Task ShouldInsertSuccessfullyIfCommandIsValid()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        
        _repositoryMock
            .Setup(x => x.UnitOfWork.CommitAsync().Result).Returns(true);
        _repositoryMock
            .Setup(x => x.ExistsEmailRegisteredAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error occurred while inserting the user"));
    }

    private InsertUserCommand CreateCommand() => new InsertUserCommand
    {
        Email = _faker.Internet.Email(),
        Password = _faker.Internet.PasswordCustom(9, 32),
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };
    
    private InsertUserCommand CreateCommand(string email) => new InsertUserCommand
    {
        Email = email,
        Password = _faker.Internet.PasswordCustom(9, 32),
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };
}