using System.Globalization;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.InsertUser.Commands;
using Application.UseCases.Users.InsertUser.Validations;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using UnitTests.Application.UseCases.Users.Shared.Extensions;

namespace UnitTests.Application.UseCases.Users;

public class InsertUserUseCaseTests
{
    private readonly ILogger<InsertUserUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IValidator<InsertUserCommand> _validator;
    private readonly IInsertUserUseCase _useCase;

    private readonly Faker _faker;

    public InsertUserUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<InsertUserUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _validator = new InsertUserCommandValidator();

        _useCase = new InsertUserUseCase(
            _loggerMock,
            _repositoryMock,
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
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
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
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfTheEmailAlreadyExists()
    {
        // Given
        const string email = "e-mail@email.com";
        var command = CreateCommand(email);
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(email, A<CancellationToken>._)).Returns(true);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("E-mail already registered")).Which.Code.Should().Be("Email");
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldInsertSuccessfullyIfCommandIsValid()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).Returns(false);
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).Returns(Task.CompletedTask);
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);
        
        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(A<string>._, A<CancellationToken>._)).Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error occurred while inserting the user"));
        
        A.CallTo(() => _repositoryMock.ExistsEmailRegisteredAsync(command.Email, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.AddAsync(A<User>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
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