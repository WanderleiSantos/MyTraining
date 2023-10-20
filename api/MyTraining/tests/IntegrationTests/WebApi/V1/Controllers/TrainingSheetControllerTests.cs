using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.UseCases.Auth.SignIn.Responses;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Responses;
using Bogus;
using FluentAssertions;
using SharedTests.Extensions;
using WebApi.V1.Models;
using Xunit;

namespace IntegrationTests.WebApi.V1.Controllers;

[Collection("mytraining collection")]
public class TrainingSheetControllerTests : IAsyncLifetime
{
    private const string UriRequestUser = "api/v1/user";
    private const string UriRequestTrainingSheet = "api/v1/trainingsheet";
    private const string UriRequestSign = "api/v1/auth/sign-in";
    private const string MediaType = "application/json";
    
    private readonly HttpClient _httpClient;
    private readonly Faker _faker;
    private readonly Func<Task> _resetDataBase;

    public TrainingSheetControllerTests(MyTrainingApiFactory fixture)
    {
        _httpClient = fixture.HttpClient;
        _resetDataBase = fixture.ResetDatabase;
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldCreateTrainingSheetSuccessfully()
    {
        //Arrange
        var user = await InsertUser();
        var accessToken = await GetAccessToken(user.Email, user.Password);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var input = InsertTainingSheetInput();
        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, MediaType);
  
        //Act
        var response = await _httpClient.PostAsync(UriRequestTrainingSheet, data);
        var content = await response.Content.ReadAsStringAsync();
        
        var sheetResponse = JsonSerializer.Deserialize<InsertTrainingSheetResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sheetResponse.Should().NotBeNull();
        sheetResponse?.Id.Should().NotBeEmpty();
        sheetResponse?.Name.Should().Be(input.Name);
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

    private InsertTrainingSheetInput InsertTainingSheetInput() => new InsertTrainingSheetInput()
    {
        Name = _faker.Random.String2(10),
        TimeExchange = _faker.Random.String2(10)
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