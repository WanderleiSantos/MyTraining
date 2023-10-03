using System.Globalization;
using Application.UseCases.Users.UpdateUser;
using Application.UseCases.Users.UpdateUser.Commands;
using Application.UseCases.Users.UpdateUser.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests.Application.UseCases.Users.Shared.Extensions;

namespace UnitTests.Application.UseCases.Users;

public class UpdateUserUseCaseTests
{
    private readonly Mock<ILogger<UpdateUserUseCase>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IUpdateUserUseCase _useCase;

    private readonly Faker _faker;

    public UpdateUserUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = new Mock<ILogger<UpdateUserUseCase>>();
        _repositoryMock = new Mock<IUserRepository>();
        _validator = new UpdateUserCommandValidator();

        _useCase = new UpdateUserUseCase(
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
        var command = new UpdateUserCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(6);
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Id' must not be empty.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Id' must not be equal to '00000000-0000-0000-0000-000000000000'.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'First Name' must not be empty.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("The length of 'First Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("FirstName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("'Last Name' must not be empty.")).Which.Code.Should().Be("LastName");
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("The length of 'Last Name' must be at least 3 characters. You entered 0 characters.")).Which.Code.Should().Be("LastName");
    }

    [Fact]
    public async Task ShouldReturnValidationErrorIfUserDoesNotExist()
    {
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeTrue();
        output.HasMessages.Should().BeTrue();
        output.Result.Should().BeNull();
        output.Messages.Should().Contain(e => e.Message.Contains("User does not exist"));
    }
    
    [Fact]
    public async Task ShouldUpdateSuccessfullyIfCommandIsValid()
    {
        // Given
        var user = CreateFakerUser();
        var updatedAt = user.UpdatedAt;
        var command = CreateCommand(user.Id);
        var cancellationToken = CancellationToken.None;
        
        _repositoryMock
            .Setup(x => x.UnitOfWork.CommitAsync().Result).Returns(true);
        _repositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.HasMessages.Should().BeFalse();
        output.Result.Should().BeNull();
        user.FirstName.Should().Be(command.FirstName);
        user.LastName.Should().Be(command.LastName);
        user.Id.Should().Be(command.Id);
        user.UpdatedAt.Should().NotBe(updatedAt);
        user.UpdatedAt.Should().BeAfter(updatedAt);
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Contains("An unexpected error occurred while update the user"));
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

    private User CreateFakerUser() => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email().ToLower(),
        _faker.Internet.PasswordCustom(9, 32)
    );
}