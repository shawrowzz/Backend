using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DataServiceLayer;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/titles")]
public class TitlesController : BaseController
{
    public TitlesController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet("{tconst}", Name = nameof(GetTitle))]
    public IActionResult GetTitle(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

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
            var omdbData = _dataService.GetOmdbData(tconst);
            if (omdbData != null)
            {
                titleDto.Plot = omdbData.Plot ?? "No plot description available";
                titleDto.Poster = omdbData.Poster ?? "";
                titleDto.Genre = omdbData.Genre ?? "";
                titleDto.Runtime = omdbData.Runtime ?? "";
                titleDto.Rated = omdbData.Rated ?? "";
                titleDto.Language = omdbData.Language ?? "";
                titleDto.Country = omdbData.Country ?? "";
                titleDto.Released = omdbData.Released ?? "";
            }
        }
        catch
        {
            // Continue without OMDb data
        }

        return Ok(MapToTitleModel(titleDto));
    }

    [HttpGet("{tconst}/omdb", Name = nameof(GetTitleOmdbData))]
    public IActionResult GetTitleOmdbData(string tconst)
    {
        try
        {
            var omdbData = _dataService.GetOmdbData(tconst);
            if (omdbData == null) return NotFound();

            var omdbDto = new OmdbDataDto
            {
                TConst = omdbData.TConst,
                Plot = omdbData.Plot ?? "",
                Poster = omdbData.Poster ?? "",
                Genre = omdbData.Genre ?? "",
                Runtime = omdbData.Runtime ?? "",
                Rated = omdbData.Rated ?? "",
                Language = omdbData.Language ?? "",
                Country = omdbData.Country ?? "",
                Released = omdbData.Released ?? "",
                Awards = omdbData.Awards ?? "",
                Writer = omdbData.Writer ?? "",
                Type = omdbData.Type ?? ""
            };

            return Ok(omdbDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching OMDb data", error = ex.Message });
        }
    }

    [HttpGet(Name = nameof(GetTitles))]
    public IActionResult GetTitles([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var totalItems = _dataService.GetTitlesCount();
        var titles = _dataService.GetTitles(page, pageSize);

        var titleModels = titles.Select(t =>
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
                }
            }
            catch
            {
                // Continue without OMDb data
            }

            return MapToTitleModel(titleDto);
        }).ToList();

        var paging = CreatePaging(nameof(GetTitles), titleModels, totalItems, page, pageSize);
        return Ok(paging);
    }

    [HttpGet("{tconst}/cast", Name = nameof(GetTitleCast))]
    public IActionResult GetTitleCast(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

        var cast = _dataService.GetTitleCast(tconst);
        var castDtos = cast.Select(c => new TitlePersonDto
        {
            TConst = c.TConst,
            NConst = c.NConst,
            Ordering = c.Ordering,
            Category = c.Category,
            Job = c.Job,
            CharacterName = c.CharacterName
        });

        return Ok(castDtos);
    }

    [HttpGet("{tconst}/genres", Name = nameof(GetTitleGenres))]
    public IActionResult GetTitleGenres(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

        var genres = _dataService.GetTitleGenres(tconst);
        var genreDtos = genres.Select(g => new TitleGenreDto
        {
            TConst = g.TConst,
            GenreName = g.GenreName
        });

        return Ok(genreDtos);
    }

    [HttpGet("{tconst}/rating", Name = nameof(GetTitleRating))]
    public IActionResult GetTitleRating(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

        var rating = _dataService.GetTitleRating(tconst);
        if (rating == null) return NotFound();

        var ratingDto = new TitleRatingDto
        {
            TConst = rating.TConst,
            AverageRating = rating.AverageRating,
            NumVotes = rating.NumVotes
        };

        return Ok(ratingDto);
    }

    [HttpGet("{tconst}/akas", Name = nameof(GetTitleAkas))]
    public IActionResult GetTitleAkas(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

        var akas = _dataService.GetAkasByTitle(tconst);
        var akaDtos = akas.Select(a => new TitleAkaDto
        {
            TitleId = a.TitleId,
            Ordering = a.Ordering,
            Title = a.Title,
            Region = a.Region,
            Language = a.Language
        });

        return Ok(akaDtos);
    }

    [HttpGet("{tconst}/episodes", Name = nameof(GetTitleEpisodes))]
    public IActionResult GetTitleEpisodes(string tconst)
    {
        var title = _dataService.GetTitle(tconst);
        if (title == null) return NotFound();

        var episodes = _dataService.GetEpisodesByTitle(tconst);
        var episodeDtos = episodes.Select(e => new TitleEpisodeDto
        {
            TConst = e.TConst,
            ParentTConst = e.ParentTConst,
            SeasonNumber = e.SeasonNumber,
            EpisodeNumber = e.EpisodeNumber
        });

        return Ok(episodeDtos);
    }


    [HttpGet("{tconst}/similar", Name = nameof(GetSimilarMovies))]
    public IActionResult GetSimilarMovies(
        string tconst,
        [FromQuery] int limit = 10)
    {
        try
        {
            var similarMovies = _dataService.FindSimilarMovies(tconst, limit);

            var result = similarMovies.Select(sm => new SimilarMovieDto
            {
                TConst = sm.TConst,
                PrimaryTitle = sm.PrimaryTitle,
                SimilarityScore = sm.SimilarityScore
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error finding similar movies",
                error = ex.Message
            });
        }
    }

    [HttpGet("{tconst}/popular-actors", Name = nameof(GetPopularActors))]
    public IActionResult GetPopularActors(string tconst)
    {
        try
        {
            var popularActors = _dataService.GetPopularActorsInMovie(tconst);

            var result = popularActors.Select(pa => new PopularActorDto
            {
                NConst = pa.NConst,
                PrimaryName = pa.PrimaryName,
                Category = pa.Category,
                CharacterName = pa.CharacterName,
                PopularityRank = pa.PopularityRank
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting popular actors",
                error = ex.Message
            });
        }
    }

    [HttpGet("{tconst}/average-rating", Name = nameof(GetAverageRating))]
    public IActionResult GetAverageRating(string tconst)
    {
        try
        {
            var averageRating = _dataService.GetAverageRating(tconst);

            if (averageRating == null)
            {
                return NotFound(new { message = "No rating found for this title" });
            }

            return Ok(new { averageRating });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting average rating",
                error = ex.Message
            });
        }
    }
}
