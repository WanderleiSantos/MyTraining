using System.Globalization;
using Application.UseCases.Exercises.UpdateExercise;
using Application.UseCases.Exercises.UpdateExercise.Commands;
using Application.UseCases.Exercises.UpdateExercise.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Errors = Core.Shared.Errors.Errors;

namespace UnitTests.Application.UseCases.Exercises;

public class UpdateExerciseUseCaseTests
{
    private readonly ILogger<UpdateExerciseUseCase> _loggerMock;
    private readonly IExerciseRepository _repositoryMock;
    private readonly IValidator<UpdateExerciseCommand> _validator;
    private readonly IUpdateExerciseUseCase _useCase;

    private readonly Faker _faker;

    public UpdateExerciseUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        _loggerMock = A.Fake<ILogger<UpdateExerciseUseCase>>();
        _repositoryMock = A.Fake<IExerciseRepository>();
        _validator = new UpdateExerciseValidator();

        _useCase = new UpdateExerciseUseCase(_loggerMock, _repositoryMock, _validator);

        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldReturnErrorValidationIfCommandIsEmpty()
    {
        //Given
        var command = new UpdateExerciseCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().HaveCount(2);
        output.Errors.Should().Contain(e => e.Description.Equals("'Id' must not be empty."));
        output.Errors.Should().Contain(e => e.Description.Equals("'Name' must not be empty."));
    }

    [Fact]
    public async Task ShouldReturnErrorIfExerciseDoesNotExist()
    {
        //Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns((Exercise?)null);

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should().Contain(Errors.Exercise.DoesNotExist);
    }

    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Throws(new Exception("ex"));

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.Errors.Should()
            .Contain(Error.Unexpected());
    }

    [Fact]
    public async Task ShouldUpdateSuccessfullyIfCommandIsValid()
    {
        var fakeExercise = CreateExercise();
        var updatedAt = fakeExercise.UpdatedAt;
        var command = CreateCommand(fakeExercise.Id);
        var cancellationToken = CancellationToken.None;
 
        A.CallTo(() => _repositoryMock.UnitOfWork.CommitAsync()).Returns(true);
        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns(fakeExercise);
        
        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);
        
        //Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
        fakeExercise.Id.Should().Be(command.Id);
        fakeExercise.Name.Should().Be(command.Name);
        fakeExercise.UpdatedAt.Should().NotBe(updatedAt);
        fakeExercise.UpdatedAt.Should().BeAfter(updatedAt);
    }

    private UpdateExerciseCommand CreateCommand(Guid fakeExerciseId) => new UpdateExerciseCommand()
    {
        Id = fakeExerciseId,
        Link = _faker.Internet.Url(),
        Name = _faker.Random.String2(10)
    };

    private UpdateExerciseCommand CreateCommand() => new UpdateExerciseCommand()
    {
        Id = _faker.Random.Guid(),
        Link = _faker.Internet.Url(),
        Name = _faker.Random.String2(10)
    };

    private Exercise CreateExercise() => new Exercise(
        _faker.Random.String2(10),
        _faker.Internet.Url(),
        _faker.Random.Guid()
    );
}