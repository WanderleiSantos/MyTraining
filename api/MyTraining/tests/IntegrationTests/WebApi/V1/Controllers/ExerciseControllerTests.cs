using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.UseCases.Auth.SignIn.Responses;
using Application.UseCases.Exercises.SearchExerciseById;
using Bogus;
using FluentAssertions;
using SharedTests.Extensions;
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
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        exerciseResponse.Should().NotBeNull();
        exerciseResponse?.Id.Should().NotBeEmpty();
        exerciseResponse?.Name.Should().Be(input.Name);
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