using System.Globalization;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Commands;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.UseCases.SeriesPlannings;

public class InsertSeriesPlanningUseCaseTests
{
    private readonly ILogger<InsertSeriesPlanningUseCase> _loggerMock;
    private readonly ISeriesPlanningRepository _repositoryMock;
    private readonly IExerciseRepository _exerciseRepositoryMock;
    private readonly IValidator<InsertSeriesPlanningCommand> _validator;
    private readonly IInsertSeriesPlanningUseCase _useCase;

    private readonly Faker _faker;
    
    public InsertSeriesPlanningUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        _loggerMock = A.Fake<ILogger<InsertSeriesPlanningUseCase>>();
        _repositoryMock = A.Fake<ISeriesPlanningRepository>();
        _exerciseRepositoryMock = A.Fake<IExerciseRepository>();
        _validator = new InsertSeriesPlanningCommandServiceValidator();

        _useCase = new InsertSeriesPlanningUseCase(_loggerMock, 
            _repositoryMock, _validator, _exerciseRepositoryMock);

        _faker = new Faker();
    }
    
    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandsIsEmpty()
    {
        //Arrange
        var command = new InsertSeriesPlanningCommand(){};
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(2);
        output.Errors.Should().Contain(e => e.Description.Equals("'Training Sheet Series Id' must not be empty."));
        output.Errors.Should().Contain(e => e.Description.Equals("'User Id' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<SeriesPlanning>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }

}