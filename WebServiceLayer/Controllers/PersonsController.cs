using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/persons")]
public class PersonsController : BaseController
{
    public PersonsController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet(Name = nameof(GetPersons))]
    public IActionResult GetPersons([FromQuery] QueryParamsModel queryParams)
    {
        var persons = _dataService.GetPersons(queryParams.Page, queryParams.PageSize)
            .Select(x => new PersonDto
            {
                NConst = x.NConst,
                PrimaryName = x.PrimaryName,
                BirthYear = x.BirthYear,
                DeathYear = x.DeathYear
            })
            .Select(MapToPersonModel);

        var totalItems = _dataService.GetPersonsCount();
        var paging = CreatePaging(nameof(GetPersons), persons, totalItems, queryParams.Page, queryParams.PageSize);

        return Ok(paging);
    }

    [HttpGet("{nconst}", Name = nameof(GetPerson))]
    public IActionResult GetPerson(string nconst)
    {
        var person = _dataService.GetPerson(nconst);
        if (person == null) return NotFound();

        var personDto = new PersonDto
        {
            NConst = person.NConst,
            PrimaryName = person.PrimaryName,
            BirthYear = person.BirthYear,
            DeathYear = person.DeathYear
        };

        return Ok(MapToPersonModel(personDto));
    }

    [HttpGet("{nconst}/titles", Name = nameof(GetPersonTitles))]
    public IActionResult GetPersonTitles(string nconst)
    {
        var titles = _dataService.GetPersonTitles(nconst)
            .Select(x => new TitlePersonDto
            {
                TConst = x.TConst,
                NConst = x.NConst,
                Ordering = x.Ordering,
                Category = x.Category,
                Job = x.Job,
                CharacterName = x.CharacterName
            })
            .ToList();

        return Ok(titles);
    }

    [HttpGet("{nconst}/professions", Name = nameof(GetPersonProfessions))]
    public IActionResult GetPersonProfessions(string nconst)
    {
        var professions = _dataService.GetProfessionsByPerson(nconst)
            .Select(x => new PersonProfessionDto
            {
                NConst = x.NConst,
                ProfessionName = x.ProfessionName
            })
            .ToList();

        return Ok(professions);
    }
}