using Application.UseCases.Exercises.InsertExercise;
using Application.UseCases.Exercises.InsertExercise.Commands;
using Application.UseCases.Exercises.InsertExercise.Validations;
using Application.UseCases.Users.InsertUser.Responses;
using Bogus;
using Core.Interfaces.Persistence.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Exercises;

public class InsertExerciseUseCaseTests
{
    private readonly Mock<ILogger<InsertExerciseUseCase>> _loggerMock;
    private readonly Mock<IExerciseRepository> _repositoryMock;
    private readonly IValidator<InsertExerciseCommand> _validator;
    private readonly IInsertExerciseUseCase _useCase;
    private readonly Faker _faker;

    public InsertExerciseUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<InsertExerciseUseCase>>();
        _repositoryMock = new Mock<IExerciseRepository>();
        _validator = new InsertExerciseCommandValidator();

        _useCase = new InsertExerciseUseCase(_loggerMock.Object, _repositoryMock.Object, _validator);

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandIsEmpty()
    {
        //Given
        var command = new InsertExerciseCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Code!.Contains("UserId")).Which.Message.Should()
            .Be("'User Id' must not be empty.");
        output.ErrorMessages.Should().Contain(e => e.Code!.Contains("Name")).Which.Message.Should()
            .Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfUserIdIsEmpty()
    {
        //Given
        var command = new InsertExerciseCommand()
            { Link = "", Name = "Supino", UserId = Guid.Empty };
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Code!.Contains("UserId")).Which.Message.Should()
            .Be("'User Id' must not be empty.");
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfNameIsEmpty()
    {
        //Given
        var command = new InsertExerciseCommand()
            { Link = "", Name = "", UserId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;
        
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);
        
        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Code!.Contains("Name")).Which.Message.Should()
            .Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task ShouldSuccessfullyIfCommandIsValid()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(x => x.UnitOfWork.CommitAsync().Result).Returns(true);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);
        
        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().NotBeNull();
        
    }

    private InsertExerciseCommand CreateCommand() => new InsertExerciseCommand()
    {
        Name = _faker.Random.String2(10),
        Link = _faker.Internet.Url(),
        UserId = _faker.Random.Guid()
    };
}