using System.Globalization;
using Application.Shared.Extensions;
using Application.Shared.Services;
using Application.UseCases.Auth.RefreshToken;
using Application.UseCases.Auth.RefreshToken.Commands;
using Application.UseCases.Auth.RefreshToken.Responses;
using Application.UseCases.Auth.RefreshToken.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedTests.Extensions;

namespace UnitTests.Application.UseCases.Auth;

public class RefreshTokenUseCaseTests
{
    private readonly ILogger<RefreshTokenUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IAuthenticationService _authenticationServiceMock;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly IRefreshTokenUseCase _useCase;

    private readonly Faker _faker;

    public RefreshTokenUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<RefreshTokenUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _authenticationServiceMock = A.Fake<IAuthenticationService>();
        _validator = new RefreshTokenCommandValidator();
        
        _useCase = new RefreshTokenUseCase(
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
        var command = new RefreshTokenCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Refresh Token' must not be empty.")).Which.Code.Should().Be("RefreshToken");
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenRefreshTokenIsWrong()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(command.RefreshToken)).Returns((false, null));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("Token is expired or User is not valid"));
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenRefreshTokenIsValidButEmailIsNotPresent()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(command.RefreshToken)).Returns((true, null));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("Token is expired or User is not valid"));
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorIfUserDoesNotExist()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(command.RefreshToken)).Returns((true, email));
        A.CallTo(() => _repositoryMock.GetByEmailAsync(email, A<CancellationToken>._)).Returns((User?)null);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist or inactive"));
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorIfUserInactive()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        var user = CreateFakeUser();
        user.Deactivate();

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(command.RefreshToken)).Returns((true, user.Email));
        A.CallTo(() => _repositoryMock.GetByEmailAsync(user.Email, A<CancellationToken>._)).Returns(user);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist or inactive"));
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
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
        var user = CreateFakeUser();
        var expectedToken = _faker.Random.String2(50);
        var expectedRefreshToken = _faker.Random.String2(50);

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(command.RefreshToken)).Returns((true, user.Email));
        A.CallTo(() => _repositoryMock.GetByEmailAsync(user.Email, A<CancellationToken>._)).Returns(user);
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(user.Id, user.Email)).Returns(expectedToken);
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(user.Id, user.Email)).Returns(expectedRefreshToken);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeAssignableTo(typeof(RefreshTokenResponse));
        ((RefreshTokenResponse)output.Result!).Token.Should().Be(expectedToken);
        ((RefreshTokenResponse)output.Result!).RefreshToken.Should().Be(expectedRefreshToken);
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
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

        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).Throws(new Exception("ex"));
        
        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error has occurred"));
        
        A.CallTo(() => _authenticationServiceMock.ValidateRefreshToken(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.GetByEmailAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateAccessToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _authenticationServiceMock.CreateRefreshToken(A<Guid>._, A<string>._)).MustNotHaveHappened();
    }
    
    private RefreshTokenCommand CreateCommand() => new RefreshTokenCommand
    {
        RefreshToken = _faker.Random.String2(50)
    };
    
    private User CreateFakeUser() => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email(),
        _faker.Internet.PasswordCustom(9, 32).HashPassword()
    );
}