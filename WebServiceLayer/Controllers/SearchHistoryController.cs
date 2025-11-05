using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users/{userId}/search-history")]
public class SearchHistoryController : BaseController
{
    public SearchHistoryController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet(Name = nameof(GetUserSearchHistory))]
    public IActionResult GetUserSearchHistory(int userId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var history = _dataService.GetUserSearchHistory(userId)
            .Select(x => new SearchHistoryDto
            {
                SearchId = x.SearchId,
                UserId = x.UserId,
                SearchQuery = x.SearchQuery,
                SearchType = x.SearchType,
                SearchedAt = x.SearchedAt
            })
            .Select(dto => new SearchHistoryModel
            {
                Url = GetUrl(nameof(GetUserSearchHistory), new { userId }),
                SearchId = dto.SearchId,
                UserId = dto.UserId,
                SearchQuery = dto.SearchQuery,
                SearchType = dto.SearchType,
                SearchedAt = dto.SearchedAt
            })
            .ToList();

        return Ok(history);
    }
}