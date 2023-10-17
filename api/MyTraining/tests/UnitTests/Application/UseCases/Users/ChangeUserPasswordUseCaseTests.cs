using System.Globalization;
using Application.Shared.Extensions;
using Application.UseCases.Users.ChangeUserPassword;
using Application.UseCases.Users.ChangeUserPassword.Commands;
using Application.UseCases.Users.ChangeUserPassword.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedTests.Extensions;
using Errors = Core.Shared.Errors.Errors;

namespace UnitTests.Application.UseCases.Users;

public class ChangeUserPasswordUseCaseTests
{
    private readonly ILogger<ChangeUserPasswordUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IValidator<ChangeUserPasswordCommand> _validator;
    private readonly IChangeUserPasswordUseCase _useCase;
    
    private readonly Faker _faker;

    public ChangeUserPasswordUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<ChangeUserPasswordUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _validator = new ChangeUserPasswordCommandValidator();

        _useCase = new ChangeUserPasswordUseCase(
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
        var command = new ChangeUserPasswordCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(6);
        output.Errors.Should().Contain(e => e.Description.Equals("'Id' must not be empty.")).Which.Code.Should().Be("Id");
        output.Errors.Should().Contain(e => e.Description.Equals("'Id' must not be equal to '00000000-0000-0000-0000-000000000000'.")).Which.Code.Should().Be("Id");
        output.Errors.Should().Contain(e => e.Description.Equals("'Old Password' must not be empty.")).Which.Code.Should().Be("OldPassword");
        output.Errors.Should().Contain(e => e.Description.Equals("The length of 'Old Password' must be at least 8 characters. You entered 0 characters.")).Which.Code.Should().Be("OldPassword");
        output.Errors.Should().Contain(e => e.Description.Equals("'New Password' must contain at least 8 characters, one number, one uppercase letter, one lowercase letter and one special character.")).Which.Code.Should().Be("NewPassword");
        output.Errors.Should().Contain(e => e.Description.Equals("'New Password' must not be empty.")).Which.Code.Should().Be("NewPassword");
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorIfUserDoesNotExist()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns((User?)null);
  
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(1);
        output.Errors.Should().Contain(Errors.User.DoesNotExist);
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorIfUserPasswordDoesNotMatch()
    {
        //Given
        var user = CreateFakeUser();
        var command = CreateCommand(user.Id);
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns(user);
  
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(1);
        output.Errors.Should().Contain(Errors.User.InvalidPassword);
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldChangeUserPasswordSuccessfully()
    {
        //Given
        var password = _faker.Internet.PasswordCustom(9, 32);
        var user = CreateFakeUser(password);
        var command = CreateCommand(user.Id, password);
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns(user);
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);
  
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().Contain(Error.Unexpected());
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    private ChangeUserPasswordCommand CreateCommand(Guid? id = null, string? oldPassword = null) => new ChangeUserPasswordCommand
    {
        Id = id ?? _faker.Random.Guid(),
        OldPassword = oldPassword ?? _faker.Internet.PasswordCustom(9, 32),
        NewPassword = _faker.Internet.PasswordCustom(9, 32)
    };
    
    private User CreateFakeUser(string? password = null) => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email(),
        password == null ? _faker.Internet.PasswordCustom(9, 32).HashPassword() : password.HashPassword() 
    );
}