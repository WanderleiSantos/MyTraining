using System.Globalization;
using Application.UseCases.Users.UpdateUser;
using Application.UseCases.Users.UpdateUser.Commands;
using Application.UseCases.Users.UpdateUser.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedTests.Extensions;

namespace UnitTests.Application.UseCases.Users;

public class UpdateUserUseCaseTests
{
    private readonly ILogger<UpdateUserUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IUpdateUserUseCase _useCase;

    private readonly Faker _faker;

    public UpdateUserUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<UpdateUserUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _validator = new UpdateUserCommandValidator();

        _useCase = new UpdateUserUseCase(
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
        var command = new UpdateUserCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(6);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Id' must not be empty.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Id' must not be equal to '00000000-0000-0000-0000-000000000000'.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'First Name' must not be empty.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'First Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Last Name' must not be empty.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("The length of 'Last Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("LastName");
        
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
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("User does not exist"));
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldUpdateSuccessfullyIfCommandIsValid()
    {
        // Given
        var user = CreateFakeUser();
        var updatedAt = user.UpdatedAt;
        var command = CreateCommand(user.Id);
        var cancellationToken = CancellationToken.None;
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns(user);
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
        user.FirstName.Should().Be(command.FirstName);
        user.LastName.Should().Be(command.LastName);
        user.Id.Should().Be(command.Id);
        user.UpdatedAt.Should().NotBe(updatedAt);
        user.UpdatedAt.Should().BeAfter(updatedAt);
        
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
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error occurred while update the user"));
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    private UpdateUserCommand CreateCommand() => new UpdateUserCommand
    {
        Id = _faker.Random.Guid(),
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };
    
    private UpdateUserCommand CreateCommand(Guid id) => new UpdateUserCommand
    {
        Id = id,
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };

    private User CreateFakeUser() => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email(),
        _faker.Internet.PasswordCustom(9, 32)
    );
}