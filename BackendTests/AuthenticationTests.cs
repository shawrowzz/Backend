using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using WebServiceLayer;
using WebServiceLayer.Models;
using Xunit;

namespace Backend.Tests;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_ValidData_ReturnsCreated()
    {
        
        var registerModel = new RegisterModel
        {
            Username = $"testuser_{Guid.NewGuid()}",
            Email = $"test{Guid.NewGuid()}@test.com",
            Password = "testpassword123"
        };

        var content = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");

       
        var response = await _client.PostAsync("/api/users/register", content);

        
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_ValidCredentials_ReturnsUser()
    {
        
        var username = $"testuser_{Guid.NewGuid()}";
        var password = "testpassword123";

        var registerModel = new RegisterModel
        {
            Username = username,
            Email = $"{username}@test.com",
            Password = password
        };

        var registerContent = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");
        await _client.PostAsync("/api/users/register", registerContent);

       
        var loginModel = new LoginModel
        {
            Username = username,
            Password = password
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/users/login", loginContent);

       
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains(username, responseContent);
    }
}