using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
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
            return BadRequest("Username and password are required");
        }

        var existingUser = _dataService.GetUser(registerModel.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        var user = _dataService.CreateUser(registerModel.Username, registerModel.Email, registerModel.Password);

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
            return BadRequest("Username and password are required");
        }

        var user = _dataService.GetUser(loginModel.Username);
        if (user == null || !_dataService.ValidateUserCredentials(loginModel.Username, loginModel.Password))
        {
            return Unauthorized("Invalid username or password");
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
        if (user == null) return NotFound();

        var userDto = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(MapToUserModel(userDto));
    }
}