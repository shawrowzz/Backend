using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : BaseController
{
    public SearchController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet("simple", Name = nameof(SimpleSearch))]
    public IActionResult SimpleSearch([FromQuery] string q, [FromQuery] QueryParamsModel queryParams)
    {
        var authResult = RequireAuthentication();
        if (authResult != null) return authResult;

        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query is required");
        }

        var user = GetAuthenticatedUser();
        var results = _dataService.SimpleSearch(q, user.UserId);

        var searchResults = results
            .Skip(queryParams.Page * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(x => new TitleDto
            {
                TConst = x.TConst,
                TitleType = x.TitleType,
                PrimaryTitle = x.PrimaryTitle,
                OriginalTitle = x.OriginalTitle,
                IsAdult = x.IsAdult,
                StartYear = x.StartYear,
                EndYear = x.EndYear,
                RuntimeMinutes = x.RuntimeMinutes
            })
            .Select(MapToTitleModel)
            .Select(model => new SearchResultModel
            {
                Url = model.Url,
                TConst = model.TConst,
                PrimaryTitle = model.PrimaryTitle,
                TitleType = model.TitleType,
                StartYear = model.StartYear
            })
            .ToList();

        return Ok(searchResults);
    }

    [HttpGet("structured", Name = nameof(StructuredSearch))]
    public IActionResult StructuredSearch([FromQuery] SearchQueryModel searchQuery, [FromQuery] QueryParamsModel queryParams)
    {
        var authResult = RequireAuthentication();
        if (authResult != null) return authResult;

        var user = GetAuthenticatedUser();
        var results = _dataService.StructuredSearch(
            searchQuery.Title ?? "",
            searchQuery.Plot ?? "",
            searchQuery.Characters ?? "",
            searchQuery.PersonNames ?? "",
            user.UserId)
            .Skip(queryParams.Page * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(x => new TitleDto
            {
                TConst = x.TConst,
                TitleType = x.TitleType,
                PrimaryTitle = x.PrimaryTitle,
                OriginalTitle = x.OriginalTitle,
                IsAdult = x.IsAdult,
                StartYear = x.StartYear,
                EndYear = x.EndYear,
                RuntimeMinutes = x.RuntimeMinutes
            })
            .Select(MapToTitleModel)
            .Select(model => new SearchResultModel
            {
                Url = model.Url,
                TConst = model.TConst,
                PrimaryTitle = model.PrimaryTitle,
                TitleType = model.TitleType,
                StartYear = model.StartYear
            })
            .ToList();

        return Ok(results);
    }

    [HttpGet("persons", Name = nameof(SearchPersons))]
    public IActionResult SearchPersons([FromQuery] string q, [FromQuery] QueryParamsModel queryParams)
    {
        var results = _dataService.SearchPersons(q)
            .Skip(queryParams.Page * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(x => new PersonDto
            {
                NConst = x.NConst,
                PrimaryName = x.PrimaryName,
                BirthYear = x.BirthYear,
                DeathYear = x.DeathYear
            })
            .Select(MapToPersonModel)
            .ToList();

        return Ok(results);
    }
}