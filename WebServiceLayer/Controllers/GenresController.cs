using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly IDataService _dataService;

    public GenresController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet(Name = nameof(GetGenres))]
    public IActionResult GetGenres()
    {
        var genres = _dataService.GetGenres()
            .Select(x => new GenreDto
            {
                GenreName = x.GenreName
            })
            .ToList();

        return Ok(genres);
    }
}