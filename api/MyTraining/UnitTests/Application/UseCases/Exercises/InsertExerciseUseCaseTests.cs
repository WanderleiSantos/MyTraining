using Application.UseCases.Exercises.InsertExercise;
using Application.UseCases.Exercises.InsertExercise.Commands;
using Application.UseCases.Exercises.InsertExercise.Validations;
using Bogus;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Exercises;

public class InsertExerciseUseCaseTests
{
    private readonly ILogger<InsertExerciseUseCase> _loggerMock;
    private readonly IExerciseRepository _repositoryMock;
    private readonly IValidator<InsertExerciseCommand> _validator;
    private readonly IInsertExerciseUseCase _useCase;
    private readonly Faker _faker;

    public InsertExerciseUseCaseTests()
    {
        _loggerMock = A.Fake<ILogger<InsertExerciseUseCase>>();
        _repositoryMock = A.Fake<IExerciseRepository>();
        _validator = new InsertExerciseCommandValidator();

        _useCase = new InsertExerciseUseCase(_loggerMock, _repositoryMock, _validator);

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
        
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);

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