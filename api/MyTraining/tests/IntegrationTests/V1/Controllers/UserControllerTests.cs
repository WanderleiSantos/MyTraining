using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using SharedTests.Extensions;
using WebApi.V1.Models;
using Xunit;

namespace IntegrationTests.V1.Controllers;

[Collection("mytraining collection")]
public class UserControllerTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly Faker _faker;
    private readonly Func<Task> _resetDataBase;
    
    public UserControllerTests(DatabaseFixture fixture)
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

        var data = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync($"api/v1/user", data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDataBase();
}