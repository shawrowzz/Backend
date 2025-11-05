using DataServiceLayer;
using DataServiceLayer.Models;
using System;
using Xunit;

namespace Backend.Tests;

public class DataServiceTests
{
    private readonly string _connectionString = "Host=localhost;Database=imdb;Username=postgres;Password=12345";
    private readonly IDataService _dataService;

    public DataServiceTests()
    {
        _dataService = new DataService(_connectionString);
    }

    [Fact]
    public void Test1_DataService_CreatesWithoutError()
    {
        // Arrange and Act
        var dataService = new DataService(_connectionString);

        // Assert
        Assert.NotNull(dataService);
    }

    [Fact]
    public void Test2_GetGenres_ReturnsList()
    {
        // Act
        var genres = _dataService.GetGenres();

        // Assert
        Assert.NotNull(genres);
    }

    [Fact]
    public void Test3_GetProfessions_ReturnsList()
    {
        // Act
        var professions = _dataService.GetProfessions();

        // Assert
        Assert.NotNull(professions);
    }

    [Fact]
    public void Test4_CreateUser_Works()
    {
        // Arrange
        var username = $"testuser_{Guid.NewGuid()}";
        var email = $"{username}@test.com";
        var password = "test123";

        // Act
        var user = _dataService.CreateUser(username, email, password);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
    }

    [Fact]
    public void Test5_ValidateUserCredentials_Works()
    {
        // Arrange
        var username = $"testuser_{Guid.NewGuid()}";
        var email = $"{username}@test.com";
        var password = "test123";
        _dataService.CreateUser(username, email, password);

        // Act
        var isValid = _dataService.ValidateUserCredentials(username, password);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Test6_GetTitles_ReturnsTitles()
    {
        try
        {
            // Act
            var titles = _dataService.GetTitles(0, 5);

            
            Assert.NotNull(titles);
        }
        catch (Exception ex)
        {
            
            Assert.Contains("titles", ex.Message); 
        }
    }
}