using System.Globalization;
using Application.Shared.Extensions;
using Application.Shared.Services;
using Application.UseCases.Auth.SignIn;
using Application.UseCases.Auth.SignIn.Commands;
using Application.UseCases.Auth.SignIn.Responses;
using Application.UseCases.Auth.SignIn.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UnitTests.Application.UseCases.Users.Shared.Extensions;

namespace UnitTests.Application.UseCases.Auth;

public class SignInUseCaseTests
{
    private readonly ILogger<SignInUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IAuthenticationService _authenticationServiceMock;
    private readonly IValidator<SignInCommand> _validator;
    private readonly ISignInUseCase _useCase;

    private readonly Faker _faker;
    
    public SignInUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<SignInUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _authenticationServiceMock = A.Fake<IAuthenticationService>();
        _validator = new SignInCommandValidator();

        _useCase = new SignInUseCase(
            _loggerMock,
            _repositoryMock,
            _validator,
            _authenticationServiceMock
        );

        _faker = new Faker();
    }
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandIsEmpty()
    {
        // Given
        var command = new SignInCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(5);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Email' must not be empty.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Email' is not a valid email address.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'Email' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("Email");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Password' must not be empty.")).Which.Code.Should().Be("Password");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'Password' must be at least 8 characters. You entered 0 characters.")).Which.Code.Should().Be("Password");
    }
    
    [Fact]
    public async Task ShouldReturnErrorIfUserDoesNotExist()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByEmailAsync(command.Email, A<CancellationToken>._)).Returns((User?)null);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist"));
        
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenPasswordIsWrong()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByEmailAsync(command.Email, A<CancellationToken>._)).Returns(CreateFakeUser());

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist"));
        
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenUserIsNotActive()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var user = CreateFakeUser(command.Email, command.Password.HashPassword());
        user.Deactivate();

        A.CallTo(() => _repositoryMock.GetByEmailAsync(command.Email, A<CancellationToken>._)).Returns(user);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("Inactive user"));
        
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldCreateTokenSuccessfully()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var expectedToken = _faker.Random.String2(50);
        var expectedRefreshToken = _faker.Random.String2(50);
        var user = CreateFakeUser(command.Email, command.Password.HashPassword()); 

        A.CallTo(() => _repositoryMock.GetByEmailAsync(command.Email, A<CancellationToken>._))
            .Returns(user);
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(user.Id, user.Email))
            .Returns(expectedToken);
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(user.Id, user.Email))
            .Returns(expectedRefreshToken);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeAssignableTo(typeof(SignInResponse));
        ((SignInResponse)output.Result!).Email.Should().Be(command.Email);
        ((SignInResponse)output.Result!).Token.Should().Be(expectedToken);
        ((SignInResponse)output.Result!).RefreshToken.Should().Be(expectedRefreshToken);
        
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._))
            .Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error has occurred"));
        
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    private SignInCommand CreateCommand() => new SignInCommand
    {
        Email = _faker.Internet.Email(),
        Password = _faker.Internet.Password()
    };
    
    private User CreateFakeUser(string? email = null, string? password = null) => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        email ?? _faker.Internet.Email(),
        password ?? _faker.Internet.PasswordCustom(9, 32).HashPassword()
    );

}