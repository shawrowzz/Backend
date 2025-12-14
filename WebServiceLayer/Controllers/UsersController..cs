// WebServiceLayer/Controllers/UsersController.cs
using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : BaseController
{
    public UsersController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpPost("register", Name = nameof(RegisterUser))]
    public IActionResult RegisterUser([FromBody] RegisterModel registerModel)
    {
        if (string.IsNullOrEmpty(registerModel.Username) || string.IsNullOrEmpty(registerModel.Password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        if (string.IsNullOrEmpty(registerModel.Email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        // Check if username already exists
        var existingUser = _dataService.GetUser(registerModel.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email already exists
        if (!string.IsNullOrEmpty(registerModel.Email))
        {
            // Note: You need to add GetUserByEmail method to IDataService if needed
        }

        // Hash the password
        var passwordHash = HashPassword(registerModel.Password);

        // Create user
        var user = _dataService.CreateUser(registerModel.Username, registerModel.Email, passwordHash);

        var userDto = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        var userModel = MapToUserModel(userDto);
        return Created(userModel.Url, userModel);
    }

    [HttpPost("login", Name = nameof(LoginUser))]
    public IActionResult LoginUser([FromBody] LoginModel loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        // Get user from database
        var user = _dataService.GetUser(loginModel.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Verify password
        var passwordHash = HashPassword(loginModel.Password);
        if (user.PasswordHash != passwordHash)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var userDto = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        var userModel = MapToUserModel(userDto);
        return Ok(userModel);
    }

    [HttpGet("{userId}", Name = nameof(GetUser))]
    public IActionResult GetUser(int userId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var user = _dataService.GetUser(userId);
        if (user == null) return NotFound(new { message = "User not found" });

        var userDto = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(MapToUserModel(userDto));
    }

    [HttpDelete("{userId}/search-history", Name = nameof(ClearUserSearchHistory))]
    public IActionResult ClearUserSearchHistory(int userId)
    {
        try
        {
            var cleared = _dataService.ClearSearchHistory(userId);
            return Ok(new { cleared = cleared });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error clearing search history",
                error = ex.Message
            });
        }
    }
    private string HashPassword(string password)
    {
        // Simple SHA256 hash for demonstration
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}