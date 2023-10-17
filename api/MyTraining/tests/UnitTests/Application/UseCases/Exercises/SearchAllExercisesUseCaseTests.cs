using System.Globalization;
using Application.Shared.Models;
using Application.UseCases.Exercises.SearchAllExercises;
using Application.UseCases.Exercises.SearchAllExercises.Commands;
using Application.UseCases.Exercises.SearchAllExercises.Responses;
using Application.UseCases.Exercises.SearchAllExercises.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Pagination;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Infrastructure.Pagination;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.UseCases.Exercises;

public class SearchAllExercisesUseCaseTests
{
    private readonly ILogger<SearchAllExercisesUseCase> _loggerMock;
    private readonly IExerciseRepository _repositoryMock;
    private readonly IValidator<SearchAllExercisesCommand> _validator;
    private readonly ISearchAllExercisesUseCase _useCase;

    private readonly Faker _faker;

    public SearchAllExercisesUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        _loggerMock = A.Fake<ILogger<SearchAllExercisesUseCase>>();
        _repositoryMock = A.Fake<IExerciseRepository>();
        _validator = new SearchAllExercisesValidator();

        _useCase = new SearchAllExercisesUseCase(
            _loggerMock, _repositoryMock, _validator
        );

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandIsEmpty()
    {
        //Given
        var command = new SearchAllExercisesCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(1);
        output.Errors.Should().Contain(e => e.Description.Equals("'User Id' must not be empty."));
    }

    [Fact]
    public async Task ShouldReturnZeroItemCountIfThereIsNoExerciseRegistered()
    {
        //Given
        var command = new SearchAllExercisesCommand() { UserId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        var paginatedExerciseMock = A.Fake<IPaginated<Exercise>>();
        A.CallTo(() => paginatedExerciseMock.Items).Returns(new List<Exercise>());
        A.CallTo(() =>
                _repositoryMock.GetAllAsync(command.UserId, command, command.PageNumber, command.PageSize, A<CancellationToken>._))
            .Returns(paginatedExerciseMock);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeTrue();
        ((PaginatedOutput<SearchAllExercisesResponse>)output.Result!).Items.Should().HaveCount(0);
        ((PaginatedOutput<SearchAllExercisesResponse>)output.Result!).PageCount.Should().Be(0);
    }

    [Fact]
    public async Task ShouldReturnListOfExercisesIfThereWasSuccess()
    {
        //Given
        var sizeList = _faker.Random.Number(1, 100);
        var listExercises = CreateFakeListExercise(sizeList);

        var command = new SearchAllExercisesCommand() { UserId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        var paginatedExercisesMock = A.Fake<IPaginated<Exercise>>();
        A.CallTo(() => paginatedExercisesMock.Items).Returns(listExercises);
        A.CallTo(() =>
                _repositoryMock.GetAllAsync(command.UserId, command, command.PageNumber, command.PageSize, A<CancellationToken>._))
            .Returns(paginatedExercisesMock);
        
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);
        
        //Assert
        output.IsValid.Should().BeTrue();
        ((PaginatedOutput<SearchAllExercisesResponse>)output.Result!).Items.Should().HaveCount(sizeList);
    }
    
    private static List<Exercise> CreateFakeListExercise(int sizeList)
    {
        var fakeExercise = new Faker<Exercise>()
            .CustomInstantiator(f => new Exercise(f.Random.String2(10), f.Internet.Url(), f.Random.Guid()));
        
        return fakeExercise.Generate(sizeList);
    }
}