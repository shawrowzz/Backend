using DataServiceLayer;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.Tests;

public class DatabaseConnectionTests
{
    private readonly string _connectionString = "Host=localhost;Database=imdb;Username=postgres;Password=12345";

    [Fact]
    public void CanConnectToDatabase()
    {
        // Arrange and Act
        using var db = new ImdbContext(_connectionString);

        // Assert
        Assert.True(db.Database.CanConnect(), "Should be able to connect to PostgreSQL database");
    }

    [Fact]
    public void DatabaseHasTitlesTable()
    {
        // Arrange
        using var db = new ImdbContext(_connectionString);

        // Act and Assert
        Assert.True(db.Titles.Any(), "Titles table should have data");
    }

    [Fact]
    public void DatabaseHasPersonsTable()
    {
        // Arrange
        using var db = new ImdbContext(_connectionString);

        // Act and Assert
        Assert.True(db.Persons.Any(), "Persons table should have data");
    }

    [Fact]
    public void DatabaseHasUsersTable()
    {
       
        using var db = new ImdbContext(_connectionString);

      
        try
        {
            var users = db.Users.ToList();
            Assert.NotNull(users); 
        }
        catch (Exception ex)
        {
        
            Assert.True(false, $"Users table query failed: {ex.Message}");
        }
    }
}