using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.UseCases.Auth.RefreshToken.Responses;
using Application.UseCases.Auth.SignIn.Responses;
using Bogus;
using FluentAssertions;
using IntegrationTests.Model;
using SharedTests.Extensions;
using WebApi.V1.Models;
using Xunit;
using Errors = Core.Shared.Errors.Errors;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IntegrationTests.WebApi.V1.Controllers;

[Collection("mytraining collection")]
public class AuthControllerTests : IAsyncLifetime
{
    private const string UriRequestUser = "api/v1/user";
    private const string UriRequestSignIn = "api/v1/auth/sign-in";
    private const string UriRequestRefreshToken = "api/v1/auth/refresh-token";
    private const string MediaType = "application/json";
    
    private readonly HttpClient _httpClient;
    private readonly Faker _faker;
    private readonly Func<Task> _resetDataBase;

    public AuthControllerTests(MyTrainingApiFactory fixture)
    {
        _httpClient = fixture.HttpClient;
        _resetDataBase = fixture.ResetDatabase;
        _faker = new Faker();
    }
    
    [Fact]
    public async Task ShouldSignInSuccessfully()
    {
        // Given
        var insertedUser = await InsertUser();
        var input = new SignInInput()
        {
            Email = insertedUser.Email,
            Password = insertedUser.Password
        };
        
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var content = await response.Content.ReadAsStringAsync();
        var signInResponse = JsonSerializer.Deserialize<SignInResponse>(content, 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        signInResponse.Should().NotBeNull();
        signInResponse?.Email.Should().Be(insertedUser.Email);
        signInResponse?.Token.Should().NotBeNull();
        signInResponse?.Token.Should().NotBeEmpty();
        signInResponse?.RefreshToken.Should().NotBeNull();
        signInResponse?.RefreshToken.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task ShouldReturnErrorSignInIfUserDoesNotExist()
    {
        // Given
        var input = new SignInInput()
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.PasswordCustom(9, 32)
        };
        
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        errorResponse?.Title.Should().Be(Errors.Authentication.InvalidCredentials.Description);
    }
    
    [Fact]
    public async Task ShouldReturnErrorSignInWhenPasswordIsWrong()
    {
        // Given
        var insertedUser = await InsertUser();
        var input = new SignInInput()
        {
            Email = insertedUser.Email,
            Password = _faker.Internet.PasswordCustom(9, 32)
        };
        
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        errorResponse?.Title.Should().Be(Errors.Authentication.InvalidCredentials.Description);
    }
    
    [Fact]
    public async Task ShouldReturnErrorSignInIfInputIsEmpty()
    {
        // Given
        var input = new SignInInput();

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorResponse?.Errors.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task ShouldReturnErrorSignInIfTheEmailIsInvalid()
    {
        // Given
        var input = new SignInInput()
        {
            Email = "e-mail.com.br",
            Password = _faker.Internet.PasswordCustom(9, 32)
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestSignIn, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorResponse?.Errors.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ShouldRefreshTokenSuccessfully()
    {
        // Given
        var insertedUser = await InsertUser();
        var refreshToken = await GetRefreshToken(insertedUser.Email, insertedUser.Password);
        var input = new RefreshTokenInput()
        {
            RefreshToken = refreshToken ?? string.Empty
        };
        
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestRefreshToken, data);
        var content = await response.Content.ReadAsStringAsync();
        var signInResponse = JsonSerializer.Deserialize<RefreshTokenResponse>(content, 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        signInResponse.Should().NotBeNull();
        signInResponse?.Token.Should().NotBeNull();
        signInResponse?.Token.Should().NotBeEmpty();
        signInResponse?.RefreshToken.Should().NotBeNull();
        signInResponse?.RefreshToken.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task ShouldReturnErrorWhenRefreshTokenIsWrong()
    {
        // Given
        var input = new RefreshTokenInput()
        {
            RefreshToken = _faker.Random.String2(50)
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestRefreshToken, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        errorResponse?.Title.Should().Be(Errors.Authentication.InvalidToken.Description);
    }
    
    [Fact]
    public async Task ShouldReturnErrorRefreshIfInputIsEmpty()
    {
        // Given
        var input = new RefreshTokenInput();

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        // Act
        var response = await _httpClient.PostAsync(UriRequestRefreshToken, data);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
            await response.Content.ReadAsStringAsync(), 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorResponse.Should().NotBeNull();
        errorResponse?.Errors.Should().HaveCount(1);
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
    
    private async Task<string?> GetRefreshToken(string email, string password)
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

        return signInResponse?.RefreshToken;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDataBase();
}