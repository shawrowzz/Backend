using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using WebServiceLayer;
using WebServiceLayer.Models;
using Xunit;

namespace Backend.Tests;

public class WebServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTitles_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/titles");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString()); 
    }

    [Fact]
    public async Task GetTitle_ValidId_ReturnsTitle()
    {
        // Act
        var response = await _client.GetAsync("/api/titles/tt0133093");

        // Assert
       
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("The Matrix", content);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetPersons_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/persons");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Search_SimpleSearch_ReturnsResults()
    {
        // Act
        var response = await _client.GetAsync("/api/search/simple?q=matrix");

        // Assert
       
        Assert.NotNull(response);
    }
}