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
    public IActionResult SimpleSearch([FromQuery] string q, [FromQuery] int page = 0, [FromQuery] int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search query is required" });
        }

        int userId = 0;
        var authUser = GetAuthenticatedUser();
        if (authUser != null)
        {
            userId = authUser.UserId;
        }

        var results = _dataService.SimpleSearch(q, userId);

        var searchResults = results
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(title =>
            {
                var titleDto = new TitleDto
                {
                    TConst = title.TConst,
                    TitleType = title.TitleType,
                    PrimaryTitle = title.PrimaryTitle,
                    OriginalTitle = title.OriginalTitle,
                    IsAdult = title.IsAdult,
                    StartYear = title.StartYear,
                    EndYear = title.EndYear,
                    RuntimeMinutes = title.RuntimeMinutes
                };

                try
                {
                    var omdbData = _dataService.GetOmdbData(title.TConst);
                    if (omdbData != null)
                    {
                        titleDto.Plot = omdbData.Plot ?? "";
                        titleDto.Poster = omdbData.Poster ?? "";
                    }
                }
                catch
                {
                    // Continue without OMDb data
                }

                return MapToTitleModel(titleDto);
            })
            .ToList();

        return Ok(searchResults);
    }

    [HttpGet("structured", Name = nameof(StructuredSearch))]
    public IActionResult StructuredSearch(
    [FromQuery] string? title = null,
    [FromQuery] string? plot = null,
    [FromQuery] string? characters = null,
    [FromQuery] string? persons = null,
    [FromQuery] int page = 0,
    [FromQuery] int pageSize = 25)
    {
        // All parameters are optional
        int userId = 0;
        var authUser = GetAuthenticatedUser();
        if (authUser != null)
        {
            userId = authUser.UserId;
        }

        try
        {
            var results = _dataService.StructuredSearch(
                title ?? "",
                plot ?? "",
                characters ?? "",
                persons ?? "",
                userId);

            var searchResults = results
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(t =>
                {
                    var titleDto = new TitleDto
                    {
                        TConst = t.TConst,
                        TitleType = t.TitleType,
                        PrimaryTitle = t.PrimaryTitle,
                        OriginalTitle = t.OriginalTitle,
                        IsAdult = t.IsAdult,
                        StartYear = t.StartYear,
                        EndYear = t.EndYear,
                        RuntimeMinutes = t.RuntimeMinutes
                    };

                    try
                    {
                        var omdbData = _dataService.GetOmdbData(t.TConst);
                        if (omdbData != null)
                        {
                            titleDto.Plot = omdbData.Plot ?? "";
                            titleDto.Poster = omdbData.Poster ?? "";
                            titleDto.Genre = omdbData.Genre ?? "";
                            titleDto.Runtime = omdbData.Runtime ?? "";
                            titleDto.Rated = omdbData.Rated ?? "";
                        }
                    }
                    catch
                    {
                        // Continue without OMDb data
                    }

                    return MapToTitleModel(titleDto);
                })
                .ToList();

            return Ok(searchResults);
        }
        catch (Exception ex)
        {
            // Return more detailed error for debugging
            return StatusCode(500, new
            {
                message = "Error performing structured search",
                error = ex.Message,
                innerException = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("persons", Name = nameof(SearchPersons))]
    public IActionResult SearchPersons([FromQuery] string q, [FromQuery] int page = 0, [FromQuery] int pageSize = 25)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search query is required" });
        }

        var results = _dataService.SearchPersons(q)
            .Skip(page * pageSize)
            .Take(pageSize)
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

    [HttpGet("co-players", Name = nameof(FindCoPlayers))]
    public IActionResult FindCoPlayers([FromQuery] string actorName)
    {
        if (string.IsNullOrWhiteSpace(actorName))
        {
            return BadRequest(new { message = "Actor name is required" });
        }

        try
        {
            var coPlayers = _dataService.FindCoPlayers(actorName);

            var result = coPlayers.Select(cp => new CoPlayerDto
            {
                NConst = cp.NConst,
                PrimaryName = cp.PrimaryName,
                CollaborationCount = cp.CollaborationCount
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error finding co-players",
                error = ex.Message
            });
        }
    }

    [HttpGet("exact", Name = nameof(ExactMatchSearch))]
    public IActionResult ExactMatchSearch(
        [FromQuery] string keywords,
        [FromQuery] int limit = 50)
    {
        if (string.IsNullOrWhiteSpace(keywords))
        {
            return BadRequest(new { message = "Keywords are required" });
        }

        try
        {
            var keywordArray = keywords.Split(',')
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToArray();

            if (!keywordArray.Any())
            {
                return BadRequest(new { message = "Valid keywords are required" });
            }

            var results = _dataService.ExactMatchSearch(keywordArray, limit);

            var searchResults = results.Select(r => new ExactMatchResultDto
            {
                TConst = r.TConst,
                PrimaryTitle = r.PrimaryTitle,
                MatchCount = r.MatchCount
            }).ToList();

            return Ok(searchResults);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error performing exact match search",
                error = ex.Message
            });
        }
    }

    [HttpGet("best", Name = nameof(BestMatchSearch))]
    public IActionResult BestMatchSearch(
        [FromQuery] string keywords,
        [FromQuery] int limit = 50)
    {
        if (string.IsNullOrWhiteSpace(keywords))
        {
            return BadRequest(new { message = "Keywords are required" });
        }

        try
        {
            var keywordArray = keywords.Split(',')
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToArray();

            if (!keywordArray.Any())
            {
                return BadRequest(new { message = "Valid keywords are required" });
            }

            var results = _dataService.BestMatchSearch(keywordArray, limit);

            var searchResults = results.Select(r => new BestMatchResultDto
            {
                TConst = r.TConst,
                PrimaryTitle = r.PrimaryTitle,
                MatchCount = r.MatchCount
            }).ToList();

            return Ok(searchResults);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error performing best match search",
                error = ex.Message
            });
        }
    }

    [HttpGet("person-words", Name = nameof(GetPersonWords))]
    public IActionResult GetPersonWords(
        [FromQuery] string personName,
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(personName))
        {
            return BadRequest(new { message = "Person name is required" });
        }

        try
        {
            var personWords = _dataService.GetPersonWords(personName, limit);

            var result = personWords.Select(pw => new PersonWordDto
            {
                Word = pw.Word,
                Frequency = pw.Frequency
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting person words",
                error = ex.Message
            });
        }
    }
}
