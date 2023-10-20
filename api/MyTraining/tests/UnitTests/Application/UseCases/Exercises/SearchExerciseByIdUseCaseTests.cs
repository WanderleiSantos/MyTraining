using System.Globalization;
using Application.UseCases.Exercises.SearchExerciseById;
using Application.UseCases.Exercises.SearchExerciseById.Commands;
using Application.UseCases.Exercises.SearchExerciseById.Responses;
using Application.UseCases.Exercises.SearchExerciseById.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.UseCases.Exercises;

public class SearchExerciseByIdUseCaseTests
{
    private readonly ILogger<SearchExerciseByIdUseCase> _loggerMock;
    private readonly IExerciseRepository _repositoryMock;
    private readonly IValidator<SearchExerciseByIdCommand> _validator;
    private readonly ISearchExerciseByIdUseCase _useCase;

    private readonly Faker _faker;

    public SearchExerciseByIdUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        _loggerMock = A.Fake<ILogger<SearchExerciseByIdUseCase>>();
        _repositoryMock = A.Fake<IExerciseRepository>();
        _validator = new SearchExerciseByIdValidator();

        _useCase = new SearchExerciseByIdUseCase(
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
        var command = new SearchExerciseByIdCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(2);
        output.Errors.Should().Contain(e => e.Description.Equals("'Id' must not be empty."));
    }

    [Fact]
    public async Task ShouldReturnNullIfExerciseDoesNotExists()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;


        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, command.UserId, A<CancellationToken>._))
            .Returns((Exercise?)null);

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

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, command.UserId, A<CancellationToken>._))
            .Returns(exercise);

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

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, command.UserId, A<CancellationToken>._))
            .Throws(new Exception("ex"));

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should()
            .Contain(Error.Unexpected());
    }

    private static SearchExerciseByIdCommand CreateCommand() => new SearchExerciseByIdCommand()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid()
    };

    private static SearchExerciseByIdCommand CreateCommand(Guid id) => new SearchExerciseByIdCommand()
    {
        Id = id,
        UserId = Guid.NewGuid()
    };

    private Exercise CreateFakeExercise() => new Exercise(
        _faker.Random.String2(10),
        _faker.Internet.Url(),
        Guid.NewGuid()
    );
}