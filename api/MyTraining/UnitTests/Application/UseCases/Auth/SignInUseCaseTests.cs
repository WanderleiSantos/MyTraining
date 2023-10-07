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
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests.Application.UseCases.Users.Shared.Extensions;

namespace UnitTests.Application.UseCases.Auth;

public class SignInUseCaseTests
{
    private readonly Mock<ILogger<SignInUseCase>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;
    private readonly IValidator<SignInCommand> _validator;
    private readonly ISignInUseCase _useCase;

    private readonly Faker _faker;
    
    public SignInUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = new Mock<ILogger<SignInUseCase>>();
        _repositoryMock = new Mock<IUserRepository>();
        _authenticationServiceMock = new Mock<IAuthenticationService>();
        _validator = new SignInCommandValidator();

        _useCase = new SignInUseCase(
            _loggerMock.Object,
            _repositoryMock.Object,
            _validator,
            _authenticationServiceMock.Object
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
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist"));
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenPasswordIsWrong()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, cancellationToken))
            .ReturnsAsync(CreateFakeUser());

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist"));
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenUserIsNotActive()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var user = CreateFakeUser(command.Email, command.Password.HashPassword());
        user.Deactivate();

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("Inactive user"));
    }
    
    [Fact]
    public async Task ShouldCreateTokenSuccessfully()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var expectedToken = _faker.Random.String2(50);
        var expectedRefreshToken = _faker.Random.String2(50);

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateFakeUser(command.Email, command.Password.HashPassword()));

        _authenticationServiceMock
            .Setup(x => x.CreateAccessToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(expectedToken);
        _authenticationServiceMock
            .Setup(x => x.CreateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(expectedRefreshToken);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeAssignableTo(typeof(SignInResponse));
        ((SignInResponse)output.Result!).Email.Should().Be(command.Email);
        ((SignInResponse)output.Result!).Token.Should().Be(expectedToken);
        ((SignInResponse)output.Result!).RefreshToken.Should().Be(expectedRefreshToken);
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error has occurred"));
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