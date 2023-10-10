using System.Globalization;
using Application.UseCases.TrainingSheets.InsertTrainingSheet;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Validations;
using Application.UseCases.TrainingSheets.Services;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.UseCases.TrainingSheets;

public class InsertTrainingSheetUseCaseTests
{
    private readonly ILogger<InsertTrainingSheetUseCase> _loggerMock;
    private readonly ITrainingSheetRepository _repositoryMock;
    private readonly IValidator<InsertTrainingSheetCommand> _validator;
    private readonly IInsertTrainingSheetUseCase _useCase;
    private readonly IDeactivateTrainingSheetService _deactivateTrainingSheetServiceMock;

    private readonly Faker _faker;

    public InsertTrainingSheetUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        _loggerMock = A.Fake<ILogger<InsertTrainingSheetUseCase>>();
        _repositoryMock = A.Fake<ITrainingSheetRepository>();
        _deactivateTrainingSheetServiceMock = A.Fake<IDeactivateTrainingSheetService>();
        _validator = new InsertTrainingSheetCommandValidator();

        _useCase = new InsertTrainingSheetUseCase(
            _loggerMock,
            _repositoryMock,
            _validator,
            _deactivateTrainingSheetServiceMock
        );

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandsIsEmpty()
    {
        //Given
        var command = new InsertTrainingSheetCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(2);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'User Id' must not be empty."));
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Name' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheet>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfUserIdIsEmpty()
    {
        //Given
        var command = new InsertTrainingSheetCommand()
            { Name = _faker.Random.String2(10), TimeExchange = null, UserId = Guid.Empty };
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'User Id' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheet>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfNameIsEmpty()
    {
        //Given
        var command = new InsertTrainingSheetCommand()
            { Name = "", TimeExchange = null, UserId = _faker.Random.Guid() };
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(1);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Name' must not be empty."));

        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheet>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldSuccessfullyIfCommandIsValid()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _deactivateTrainingSheetServiceMock.Deactivate(command.UserId, A<CancellationToken>._))
            .Returns(Task.CompletedTask);
        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheet>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().NotBeNull();
        A.CallTo(() => _repositoryMock.AddAsync(A<TrainingSheet>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _deactivateTrainingSheetServiceMock.Deactivate(A<Guid>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).MustHaveHappenedOnceExactly();
    }
    
    private InsertTrainingSheetCommand CreateCommand() => new InsertTrainingSheetCommand
    {
        Name = _faker.Random.String2(10), TimeExchange = null, UserId = _faker.Random.Guid()
    };
}