using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Shared.Models;
using Application.UseCases.Auth.SignIn.Responses;
using Application.UseCases.Exercises.InsertExercise.Responses;
using Application.UseCases.Exercises.SearchAllExercises.Responses;
using Application.UseCases.Exercises.SearchExerciseById.Responses;
using Bogus;
using FluentAssertions;
using SharedTests.Extensions;
using WebApi.Shared.Error;
using WebApi.V1.Models;
using Xunit;

namespace IntegrationTests.WebApi.V1.Controllers;

[Collection("mytraining collection")]
public class ExerciseControllerTests : IAsyncLifetime
{
    private const string UriRequestUser = "api/v1/user";
    private const string UriRequestExercise = "api/v1/exercise";
    private const string UriRequestSign = "api/v1/auth/sign-in";
    private const string MediaType = "application/json";

    private readonly HttpClient _httpClient;
    private readonly Faker _faker;
    private readonly Func<Task> _resetDataBase;

    public ExerciseControllerTests(MyTrainingApiFactory fixture)
    {
        _httpClient = fixture.HttpClient;
        _resetDataBase = fixture.ResetDatabase;
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldCreateExerciseSuccessfully()
    {
        //Arrange
        var user = await InsertUser();
        var accessToken = await GetAccessToken(user.Email, user.Password);
        var input = InsertExerciseInput();
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await _httpClient.PostAsync(UriRequestExercise, data);
        var content = await response.Content.ReadAsStringAsync();

        var exerciseResponse = JsonSerializer.Deserialize<InsertExerciseResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        exerciseResponse.Should().NotBeNull();
        exerciseResponse?.Id.Should().NotBeEmpty();
        exerciseResponse?.Name.Should().Be(input.Name);
    }


    [Fact]
    public async Task ShouldUpdatedExerciseSuccessfully()
    {
        //Arrange
        var user = await InsertUser();
        var accessToken = await GetAccessToken(user.Email, user.Password);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var exerciseInserted = await InsertExercise();

        var inputUpdateExercise = new UpdateExerciseInput()
        {
            Name = _faker.Random.String2(15),
            Link = _faker.Internet.Url()
        };

        var data = new StringContent(JsonSerializer.Serialize(inputUpdateExercise), Encoding.UTF8, MediaType);

        //Act
        var response = await _httpClient.PatchAsync($"{UriRequestExercise}/{exerciseInserted?.Id}", data);
        var searchedExercise = await SearchExercise(exerciseInserted?.Id);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        searchedExercise?.Name.Should().Be(inputUpdateExercise.Name);
    }

    [Fact]
    public async Task ShouldSearchExerciseByIdSuccessfully()
    {
        //Arrange
        var user = await InsertUser();
        var accessToken = await GetAccessToken(user.Email, user.Password);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var exerciseInserted = await InsertExercise();

        //Act
        var response = await _httpClient.GetAsync($"{UriRequestExercise}/{exerciseInserted?.Id}");
        var content = await response.Content.ReadAsStringAsync();

        var searchedExercise = JsonSerializer.Deserialize<SearchExerciseByIdResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        searchedExercise?.Id.Should().Be(exerciseInserted?.Id);
        searchedExercise?.Name.Should().Be(exerciseInserted?.Name);
    }

    [Fact]
    public async Task ShouldSearchAllExerciseSuccessfully()
    {
        //Arrange
        var user = await InsertUser();

        var accessToken = await GetAccessToken(user.Email, user.Password);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        await InsertExercise();

        //Act
        var response = await _httpClient.GetAsync($"{UriRequestExercise}");
        var content = await response.Content.ReadAsStringAsync();

        var paginatedResponse =
            JsonSerializer.Deserialize<PaginatedOutput<SearchAllExercisesResponse>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paginatedResponse?.Items.Should().NotBeNull();
        paginatedResponse?.PageSize.Should().Be(50);
        paginatedResponse?.Items.Should().HaveCount(50);
        paginatedResponse?.TotalItemCount.Should().Be(132);
    }

    [Fact]
    public async Task ShouldReturnErrorCreateExerciseIfInputIsEmpty()
    {
        // Given
        var user = await InsertUser();
        var accessToken = await GetAccessToken(user.Email, user.Password);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


        var input = new InsertExerciseInput();
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);

        //Act
        var response = await _httpClient.PostAsync(UriRequestExercise, data);
        var errorMessages = JsonSerializer.Deserialize<List<ErrorOutput>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessages.Should().NotBeNull();
    }


    private async Task<string?> GetAccessToken(string email, string password)
    {
        var input = new SignInInput()
        {
            Email = email,
            Password = password
        };

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);
        var response = await _httpClient.PostAsync(UriRequestSign, data);
        var content = await response.Content.ReadAsStringAsync();
        var signInResponse = JsonSerializer.Deserialize<SignInResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return signInResponse?.Token;
    }

    private InsertExerciseInput InsertExerciseInput() => new InsertExerciseInput()
    {
        Name = _faker.Random.String2(10),
        Link = _faker.Internet.Url()
    };

    private async Task<InsertExerciseResponse?> InsertExercise()
    {
        var input = new InsertExerciseInput()
        {
            Name = _faker.Random.String2(10),
            Link = _faker.Internet.Url()
        };

        var response = await _httpClient.PostAsync(UriRequestExercise,
            new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType));

        var content = await response.Content.ReadAsStringAsync();

        var exerciseResponse = JsonSerializer.Deserialize<InsertExerciseResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return exerciseResponse;
    }

    private async Task<SearchExerciseByIdResponse?> SearchExercise(Guid? id)
    {
        var response = await _httpClient.GetAsync($"{UriRequestExercise}/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<SearchExerciseByIdResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<InsertUserInput> InsertUser()
    {
        var input = new InsertUserInput()
        {
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.PasswordCustom(9, 32)
        };

        await _httpClient.PostAsync(UriRequestUser,
            new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType));

        return input;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDataBase();
}