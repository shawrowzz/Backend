using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet(Name = nameof(GetTitles))]
    public IActionResult GetTitles([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var totalItems = _dataService.GetTitlesCount();
        var titles = _dataService.GetTitles(page, pageSize);

        var titleDtos = titles.Select(t => new TitleDto
        {
            TConst = t.TConst,
            TitleType = t.TitleType,
            PrimaryTitle = t.PrimaryTitle,
            OriginalTitle = t.OriginalTitle,
            IsAdult = t.IsAdult,
            StartYear = t.StartYear,
            EndYear = t.EndYear,
            RuntimeMinutes = t.RuntimeMinutes
        });

        var titleModels = titleDtos.Select(MapToTitleModel);
        var paging = CreatePaging(nameof(GetTitles), titleModels, totalItems, page, pageSize);

        return Ok(paging);
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

        return Ok(MapToTitleModel(titleDto));
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
}