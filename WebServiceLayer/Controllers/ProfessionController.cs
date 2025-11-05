using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/professions")]
public class ProfessionsController : ControllerBase
{
    private readonly IDataService _dataService;

    public ProfessionsController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet(Name = nameof(GetProfessions))]
    public IActionResult GetProfessions()
    {
        var professions = _dataService.GetProfessions()
            .Select(x => new ProfessionDto
            {
                ProfessionName = x.ProfessionName
            })
            .ToList();

        return Ok(professions);
    }
}
