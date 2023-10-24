using System.Globalization;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.UseCases.TrainingSheetSerie;

public class InsertTrainingSheetSeriesUseCaseTests
{
    private readonly ILogger<InsertTrainingSheetSeriesUseCase> _loggerMock;
    private readonly ITrainingSheetSeriesRepository _repositoryMock;
    private readonly IValidator<InsertTrainingSheetSeriesCommand> _validator;
    private readonly IInsertTrainingSheetSeriesUseCase _useCase;

    private readonly Faker _faker;

    public InsertTrainingSheetSeriesUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        _loggerMock = A.Fake<ILogger<InsertTrainingSheetSeriesUseCase>>();
        _repositoryMock = A.Fake<ITrainingSheetSeriesRepository>();
        _validator = new InsertTrainingSheetSeriesCommandValidator();

        _useCase = new InsertTrainingSheetSeriesUseCase(_loggerMock, _repositoryMock, _validator);

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandsIsEmpty()
    {
        //Arrange
        var command = new InsertTrainingSheetSeriesCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(2);
        output.Errors.Should().Contain(e => e.Description.Equals("'Training Sheet Id' must not be empty."));
        output.Errors.Should().Contain(e => e.Description.Equals("'Name' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheetSeries>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfTrainingSheetIdIsEmpty()
    {
        //Given
        var command = new InsertTrainingSheetSeriesCommand()
            { Name = _faker.Random.String2(10), TrainingSheetId = Guid.Empty };
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(1);
        output.Errors.Should().Contain(e => e.Description.Equals("'Training Sheet Id' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheetSeries>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfNameIsEmpty()
    {
        //Given
        var command = new InsertTrainingSheetSeriesCommand()
            { Name = "", TrainingSheetId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(1);
        output.Errors.Should().Contain(e => e.Description.Equals("'Name' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheetSeries>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldSuccessfullyIfCommandIsValid()
    {
        //Given
        var command = new InsertTrainingSheetSeriesCommand()
            { Name = _faker.Random.String2(10), TrainingSheetId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

 
        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheetSeries>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().NotBeNull();
        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheetSeries>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustHaveHappenedOnceExactly();
    }

}