using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users/{userId}/ratings")]
public class UserRatingsController : BaseController
{
    public UserRatingsController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet(Name = nameof(GetUserRatings))]
    public IActionResult GetUserRatings(int userId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var ratings = _dataService.GetUserRatings(userId)
            .Select(x => new UserRatingDto
            {
                RatingId = x.RatingId,
                UserId = x.UserId,
                TitleTConst = x.TitleTConst,
                Rating = x.Rating,
                RatedAt = x.RatedAt
            })
            .Select(dto => new UserRatingModel
            {
                Url = GetUrl(nameof(GetUserRating), new { userId, ratingId = dto.RatingId }),
                RatingId = dto.RatingId,
                UserId = dto.UserId,
                TitleTConst = dto.TitleTConst,
                Rating = dto.Rating,
                RatedAt = dto.RatedAt
            })
            .ToList();

        return Ok(ratings);
    }

    [HttpGet("{ratingId}", Name = nameof(GetUserRating))]
    public IActionResult GetUserRating(int userId, int ratingId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var rating = _dataService.GetUserRating(userId, ratingId.ToString());
        if (rating == null) return NotFound();

        var ratingDto = new UserRatingDto
        {
            RatingId = rating.RatingId,
            UserId = rating.UserId,
            TitleTConst = rating.TitleTConst,
            Rating = rating.Rating,
            RatedAt = rating.RatedAt
        };

        var ratingModel = new UserRatingModel
        {
            Url = GetUrl(nameof(GetUserRating), new { userId, ratingId }),
            RatingId = ratingDto.RatingId,
            UserId = ratingDto.UserId,
            TitleTConst = ratingDto.TitleTConst,
            Rating = ratingDto.Rating,
            RatedAt = ratingDto.RatedAt
        };

        return Ok(ratingModel);
    }

    [HttpPost(Name = nameof(CreateUserRating))]
    public IActionResult CreateUserRating(int userId, [FromBody] CreateRatingModel createRating)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var existingRating = _dataService.GetUserRating(userId, createRating.TitleTConst);
        if (existingRating != null)
        {
            return BadRequest("You have already rated this title");
        }

        var rating = _dataService.CreateUserRating(userId, createRating.TitleTConst, createRating.Rating);

        var ratingDto = new UserRatingDto
        {
            RatingId = rating.RatingId,
            UserId = rating.UserId,
            TitleTConst = rating.TitleTConst,
            Rating = rating.Rating,
            RatedAt = rating.RatedAt
        };

        var ratingModel = new UserRatingModel
        {
            Url = GetUrl(nameof(GetUserRating), new { userId, ratingId = ratingDto.RatingId }),
            RatingId = ratingDto.RatingId,
            UserId = ratingDto.UserId,
            TitleTConst = ratingDto.TitleTConst,
            Rating = ratingDto.Rating,
            RatedAt = ratingDto.RatedAt
        };

        return Created(ratingModel.Url, ratingModel);
    }

    [HttpDelete("{ratingId}", Name = nameof(DeleteUserRating))]
    public IActionResult DeleteUserRating(int userId, int ratingId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        if (_dataService.DeleteUserRating(ratingId))
        {
            return NoContent();
        }

        return NotFound();
    }
}