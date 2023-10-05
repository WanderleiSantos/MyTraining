using System.Globalization;
using Application.UseCases.Exercises.SearchExerciseById;
using Application.UseCases.Exercises.SearchExerciseById.Commands;
using Application.UseCases.Exercises.SearchExerciseById.Responses;
using Application.UseCases.Exercises.SearchExerciseById.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Exercises;

public class SearchExerciseByIdUseCaseTests
{
    private readonly Mock<ILogger<SearchExerciseByIdUseCase>> _loggerMock;
    private readonly Mock<IExerciseRepository> _repositoryMock;
    private readonly IValidator<SearchExerciseByIdCommand> _validator;
    private readonly ISearchExerciseByIdUseCase _useCase;

    private readonly Faker _faker;

    public SearchExerciseByIdUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        _loggerMock = new Mock<ILogger<SearchExerciseByIdUseCase>>();
        _repositoryMock = new Mock<IExerciseRepository>();
        _validator = new SearchExerciseByIdValidator();

        _useCase = new SearchExerciseByIdUseCase(
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
        var command = new SearchExerciseByIdCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Id' must not be empty."));
    }

    [Fact]
    public async Task ShouldReturnNullIfExerciseDoesNotExists()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Act
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldSearchExerciseByIdSucessfully()
    {
        //Given
        var exercise = CreateFakeExercise();
        var command = CreateCommand(exercise.Id);
        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id, cancellationToken))
            .ReturnsAsync(exercise);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeAssignableTo(typeof(SearchExerciseByIdResponse));
        ((SearchExerciseByIdResponse)output.Result!).Id.Should().Be(exercise.Id);
    }

    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id, cancellationToken))
            .Throws(new Exception("ex"));
        
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);
        
        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should()
            .Contain(e => e.Message.Equals("An unexpected error occurred while searching the exercise."));
    }

    private static SearchExerciseByIdCommand CreateCommand() => new SearchExerciseByIdCommand()
    {
        Id = Guid.NewGuid()
    };

    private static SearchExerciseByIdCommand CreateCommand(Guid id) => new SearchExerciseByIdCommand()
    {
        Id = id
    };

    private Exercise CreateFakeExercise() => new Exercise(
        _faker.Random.String2(10),
        _faker.Internet.Url(),
        Guid.NewGuid()
    );
}