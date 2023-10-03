using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using MyTraining.Application.UseCases.Users.InsertUser;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;
using MyTraining.Application.UseCases.Users.InsertUser.Validations;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace UnitTests.Application.UseCases.Users;

public class InsertUserUseCaseTests
{
    private readonly Mock<ILogger<InsertUserUseCase>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly IValidator<InsertUserCommand> _validator;
    private readonly IInsertUserUseCase _useCase;

    private readonly Faker _faker;

    public InsertUserUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<InsertUserUseCase>>();
        _repositoryMock = new Mock<IUserRepository>();
        _validator = new InsertUserCommandValidator();

        _useCase = new InsertUserUseCase(
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
        var command = new InsertUserCommand();
        var cancelationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancelationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(8);
    }


    private InsertUserCommand CreateCommand() => new InsertUserCommand
    {
        Email = _faker.Internet.Email(),
        Password = _faker.Random.String2(10),
        FirstName = _faker.Random.String2(10),
        LastName = _faker.Random.String2(10)
    };
}