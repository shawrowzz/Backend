using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

public class BaseController : ControllerBase
{
    protected readonly IDataService _dataService;
    protected readonly LinkGenerator _generator;

    public BaseController(IDataService dataService, LinkGenerator generator)
    {
        _dataService = dataService;
        _generator = generator;
    }

    protected DataServiceLayer.Models.User GetAuthenticatedUser()
    {
       
        if (HttpContext.Items.TryGetValue("User", out var userObj) && userObj is DataServiceLayer.Models.User user)
        {
            return user;
        }

       
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader))
        {
            var username = authHeader.Trim();
            
            var authenticatedUser = _dataService.GetUser(username);
            if (authenticatedUser != null)
            {
                HttpContext.Items["User"] = authenticatedUser;
                return authenticatedUser;
            }
        }

        return null;
    }

    protected IActionResult RequireAuthentication()
    {
        var user = GetAuthenticatedUser();
        if (user == null) return Unauthorized("Authentication required");
        return null;
    }

    protected IActionResult RequireUserMatch(int userId)
    {
        var authResult = RequireAuthentication();
        if (authResult != null) return authResult;

        var user = GetAuthenticatedUser();
        if (user.UserId != userId) return Unauthorized("User mismatch");
        return null;
    }

    protected PagingModel<T> CreatePaging<T>(string endpointName, IEnumerable<T> items, int totalItems, int page, int pageSize)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return new PagingModel<T>
        {
            First = GetUrl(endpointName, new { page = 0, pageSize }),
            Prev = page > 0 ? GetUrl(endpointName, new { page = page - 1, pageSize }) : null,
            Next = page < totalPages - 1 ? GetUrl(endpointName, new { page = page + 1, pageSize }) : null,
            Last = GetUrl(endpointName, new { page = totalPages - 1, pageSize }),
            Current = GetUrl(endpointName, new { page, pageSize }),
            NumberOfPages = totalPages,
            NumberOfItems = totalItems,
            Items = items.ToList()
        };
    }

    protected string GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }

    protected TitleModel MapToTitleModel(TitleDto dto)
    {
        return new TitleModel
        {
            Url = GetUrl(nameof(TitlesController.GetTitle), new { tconst = dto.TConst }),
            TConst = dto.TConst,
            TitleType = dto.TitleType,
            PrimaryTitle = dto.PrimaryTitle,
            OriginalTitle = dto.OriginalTitle,
            IsAdult = dto.IsAdult,
            StartYear = dto.StartYear,
            EndYear = dto.EndYear,
            RuntimeMinutes = dto.RuntimeMinutes
        };
    }

    protected PersonModel MapToPersonModel(PersonDto dto)
    {
        return new PersonModel
        {
            Url = GetUrl(nameof(PersonsController.GetPerson), new { nconst = dto.NConst }),
            NConst = dto.NConst,
            PrimaryName = dto.PrimaryName,
            BirthYear = dto.BirthYear,
            DeathYear = dto.DeathYear
        };
    }

    protected UserModel MapToUserModel(UserDto dto)
    {
        return new UserModel
        {
            Url = GetUrl(nameof(UsersController.GetUser), new { userId = dto.UserId }),
            UserId = dto.UserId,
            Username = dto.Username,
            Email = dto.Email,
            CreatedAt = dto.CreatedAt
        };
    }
}