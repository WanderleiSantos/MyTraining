using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.UseCases.Auth.SignIn.Responses;
using Application.UseCases.Users.SearchUserById.Responses;
using Bogus;
using FluentAssertions;
using IntegrationTests.Model;
using SharedTests.Extensions;
using WebApi.V1.Models;
using Xunit;
using Errors = Core.Shared.Errors.Errors;

namespace IntegrationTests.WebApi.V1.Controllers;

[Collection("mytraining collection")]
public class UserControllerTests : IAsyncLifetime
{
    private const string UriRequestUser = "api/v1/user";
    private const string UriRequestSignIn = "api/v1/auth/sign-in";
    private const string MediaType = "application/json";
    
    private readonly HttpClient _httpClient;
    private readonly Faker _faker;
    private readonly Func<Task> _resetDataBase;

    public UserControllerTests(MyTrainingApiFactory fixture)
    {
        _httpClient = fixture.HttpClient;
        _resetDataBase = fixture.ResetDatabase;
        _faker = new Faker();
    }
    
    [Fact]
    public async Task ShouldCreateUserSuccessfully()
    {
        // Given
        var input = new InsertUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.PasswordCustom(9, 32)
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestUser, data);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().BeEmpty();
        response.Headers.Location?.AbsolutePath.Should().Be($"/{UriRequestUser}");
    }
    
    [Fact]
    public async Task ShouldReturnErrorCreateUserIfTheEmailAlreadyExists()
    {
        // Given
        var input = new InsertUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.PasswordCustom(9, 32)
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        await _httpClient.PostAsync(UriRequestUser, data);
        
        // Act
        var response = await _httpClient.PostAsync(UriRequestUser, data);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        content.Should().Contain(Errors.User.DuplicateEmail.Description);
    }
    
    [Fact]
    public async Task ShouldReturnErrorCreateUserIfInputIsEmpty()
    {
        // Given
        var input = new InsertUserInput();

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestUser, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorResponse?.Errors.Should().HaveCount(4);
    }
    
    [Fact]
    public async Task ShouldReturnErrorCreateUserIfTheEmailIsInvalid()
    {
        // Given
        var input = new InsertUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
            Email = "e-mail.com.br",
            Password = _faker.Internet.PasswordCustom(9, 32)
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        await _httpClient.PostAsync(UriRequestUser, data);
        
        var response = await _httpClient.PostAsync(UriRequestUser, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorResponse?.Errors.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ShouldSearchUserSuccessfully()
    {
        // Given
        var insertedUser = await InsertUser();
        var accessToken = await GetAccessToken(insertedUser.Email, insertedUser.Password);
        
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        // Act
        var response = await _httpClient.GetAsync(UriRequestUser);
        var content = await response.Content.ReadAsStringAsync();
        var searchUserByIdResponse = JsonSerializer.Deserialize<SearchUserByIdResponse>(content, 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        searchUserByIdResponse.Should().NotBeNull();
        searchUserByIdResponse?.Id.Should().NotBeEmpty();
        searchUserByIdResponse?.FirstName.Should().Be(insertedUser.FirstName);
        searchUserByIdResponse?.LastName.Should().Be(insertedUser.LastName);
        searchUserByIdResponse?.Email.Should().Be(insertedUser.Email);
        searchUserByIdResponse?.Active.Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldReturnsUnauthorizedWhenTryingSearchUser()
    {
        // Given
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // Act
        var response = await _httpClient.GetAsync(UriRequestUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ShouldUpdateUserSuccessfully()
    {
        // Given
        var insertedUser = await InsertUser();
        var accessToken = await GetAccessToken(insertedUser.Email, insertedUser.Password);
        
        var input = new UpdateUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);
        
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        // Act
        var response = await _httpClient.PutAsync(UriRequestUser, data);
        var content = await response.Content.ReadAsStringAsync();
        var searchUser = await SearchUser();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        content.Should().BeEmpty();
        searchUser?.FirstName.Should().Be(input.FirstName);
        searchUser?.LastName.Should().Be(input.LastName);
    }
    
    [Fact]
    public async Task ShouldReturnsUnauthorizedWhenTryingUpdateUser()
    {
        // Given
        var input = new UpdateUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // Act
        var response = await _httpClient.PutAsync(UriRequestUser, data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    private async Task<string?> GetAccessToken(string email, string password)
    {
        var input = new SignInInput()
        {
            Email = email,
            Password = password
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var content = await response.Content.ReadAsStringAsync();
        var signInResponse = JsonSerializer.Deserialize<SignInResponse>(content, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        return signInResponse?.Token;
    }

    private async Task<InsertUserInput> InsertUser()
    {
        var input = new InsertUserInput()
        {
            FirstName = _faker.Random.String2(10),
            LastName = _faker.Random.String2(10),
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.PasswordCustom(9, 32)
        };
        
        await _httpClient.PostAsync(UriRequestUser, 
            new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType));
        
        return input;
    }

    private async Task<SearchUserByIdResponse?> SearchUser()
    {
        var response = await _httpClient.GetAsync(UriRequestUser);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SearchUserByIdResponse>(content, 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDataBase();
}