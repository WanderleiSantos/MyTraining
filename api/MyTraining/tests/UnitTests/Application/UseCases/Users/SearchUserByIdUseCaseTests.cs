using System.Globalization;
using Application.Shared.Models;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.SearchUserById.Responses;
using Application.UseCases.Users.SearchUserById.Validations;
using Bogus;
using Core.Common.Errors;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedTests.Extensions;

namespace UnitTests.Application.UseCases.Users;

public class SearchUserByIdUseCaseTests
{
    private readonly ILogger<SearchUserByIdUseCase> _loggerMock;
    private readonly IUserRepository _repositoryMock;
    private readonly IValidator<SearchUserByIdCommand> _validator;
    private readonly ISearchUserByIdUseCase _useCase;

    private readonly Faker _faker;
    
    public SearchUserByIdUseCaseTests()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        _loggerMock = A.Fake<ILogger<SearchUserByIdUseCase>>();
        _repositoryMock = A.Fake<IUserRepository>();
        _validator = new SearchUserByIdCommandValidator();

        _useCase = new SearchUserByIdUseCase(
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
        var command = new SearchUserByIdCommand();
        var cancellationToken = CancellationToken.None;

        //Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        //Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().HaveCount(2);
        output.ErrorMessages.Should().Contain(e => e.Description.Equals("'Id' must not be empty.")).Which.Code.Should().Be("Id");
        output.ErrorMessages.Should().Contain(e => e.Description.Equals("'Id' must not be equal to '00000000-0000-0000-0000-000000000000'.")).Which.Code.Should().Be("Id");
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldReturnNullIfUserDoesNotExist()
    {
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns((User?)null);

        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        output.IsValid.Should().BeFalse();
        output.Result.Should().BeNull();
        output.ErrorType.Should().Be(ErrorType.NotFound);
        output.ErrorMessages.Should().Contain(e => e.Description.Equals("User does not exist"));
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldSearchUserByIdSuccessfully()
    {
        // Given
        var user = CreateFakeUser();
        var command = CreateCommand(user.Id);
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(command.Id, A<CancellationToken>._)).Returns(user);

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
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldLogErrorWhenThrowException()
    {
        // Given
        var command = CreateCommand();
        var cancellationToken = CancellationToken.None;

        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).Throws(new Exception("ex"));

        // Act
        var output = await _useCase.ExecuteAsync(command, cancellationToken);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(e => e.Description.Equals("An unexpected error occurred while searching the user."));
        
        A.CallTo(() => _repositoryMock.GetByIdAsync(A<Guid>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
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