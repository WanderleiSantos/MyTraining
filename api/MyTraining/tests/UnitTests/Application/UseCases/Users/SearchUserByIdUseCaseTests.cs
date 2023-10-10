using System.Globalization;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.SearchUserById.Responses;
using Application.UseCases.Users.SearchUserById.Validations;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SharedTests.Extensions;

namespace UnitTests.Application.UseCases.Users;

public class SearchUserByIdUseCaseTests
{
    private readonly Mock<ILogger<SearchUserByIdUseCase>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly IValidator<SearchUserByIdCommand> _validator;
    private readonly ISearchUserByIdUseCase _useCase;

    private readonly Faker _faker;
    
    public SearchUserByIdUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = new Mock<ILogger<SearchUserByIdUseCase>>();
        _repositoryMock = new Mock<IUserRepository>();
        _validator = new SearchUserByIdCommandValidator();

        _useCase = new SearchUserByIdUseCase(
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
        var command = new SearchUserByIdCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(2);
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Id' must not be empty.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("'Id' must not be equal to '00000000-0000-0000-0000-000000000000'.")).Which.Code.Should().Be("Id");
    }
    
    [Fact]
    public async Task ShouldReturnNullIfUserDoesNotExist()
    {
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeTrue();
        output.Result.Should().BeNull();
        output.HasMessages.Should().BeTrue();
        output.Messages.Should().Contain(e => e.Message.Equals("User does not exist"));
    }
    
    [Fact]
    public async Task ShouldSearchUserByIdSuccessfully()
    {
        // Given
        var user = CreateFakeUser();
        var command = CreateCommand(user.Id);
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, cancellationToken))
            .ReturnsAsync(user);

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeAssignableTo(typeof(SearchUserByIdResponse));
        ((SearchUserByIdResponse)output.Result!).Id.Should().Be(user.Id);
        ((SearchUserByIdResponse)output.Result!).FirstName.Should().Be(user.FirstName);
        ((SearchUserByIdResponse)output.Result!).LastName.Should().Be(user.LastName);
        ((SearchUserByIdResponse)output.Result!).Email.Should().Be(user.Email);
        ((SearchUserByIdResponse)output.Result!).Active.Should().Be(user.Active);
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Message.Equals("An unexpected error occurred while searching the user."));
    }
    
    private static SearchUserByIdCommand CreateCommand() => new SearchUserByIdCommand
    {
        Id = Guid.NewGuid()
    };
    
    private static SearchUserByIdCommand CreateCommand(Guid id) => new SearchUserByIdCommand
    {
        Id = id
    };
    
    private User CreateFakeUser() => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email(),
        _faker.Internet.PasswordCustom(9, 32)
    );
}