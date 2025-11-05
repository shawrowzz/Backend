using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users/{userId}/bookmarks")]
public class UserBookmarksController : BaseController
{
    public UserBookmarksController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet(Name = nameof(GetUserBookmarks))]
    public IActionResult GetUserBookmarks(int userId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var bookmarks = _dataService.GetUserBookmarks(userId)
            .Select(x => new UserBookmarkDto
            {
                BookmarkId = x.BookmarkId,
                UserId = x.UserId,
                TitleTConst = x.TitleTConst,
                PersonNConst = x.PersonNConst,
                BookmarkType = x.BookmarkType,
                Folder = x.Folder,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .Select(dto => new UserBookmarkModel
            {
                Url = GetUrl(nameof(GetUserBookmark), new { userId, bookmarkId = dto.BookmarkId }),
                BookmarkId = dto.BookmarkId,
                UserId = dto.UserId,
                TitleTConst = dto.TitleTConst,
                PersonNConst = dto.PersonNConst,
                BookmarkType = dto.BookmarkType,
                Folder = dto.Folder,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt
            })
            .ToList();

        return Ok(bookmarks);
    }

    [HttpGet("{bookmarkId}", Name = nameof(GetUserBookmark))]
    public IActionResult GetUserBookmark(int userId, int bookmarkId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var bookmarks = _dataService.GetUserBookmarks(userId);
        var bookmark = bookmarks.FirstOrDefault(x => x.BookmarkId == bookmarkId);
        if (bookmark == null) return NotFound();

        var bookmarkDto = new UserBookmarkDto
        {
            BookmarkId = bookmark.BookmarkId,
            UserId = bookmark.UserId,
            TitleTConst = bookmark.TitleTConst,
            PersonNConst = bookmark.PersonNConst,
            BookmarkType = bookmark.BookmarkType,
            Folder = bookmark.Folder,
            Notes = bookmark.Notes,
            CreatedAt = bookmark.CreatedAt
        };

        var bookmarkModel = new UserBookmarkModel
        {
            Url = GetUrl(nameof(GetUserBookmark), new { userId, bookmarkId }),
            BookmarkId = bookmarkDto.BookmarkId,
            UserId = bookmarkDto.UserId,
            TitleTConst = bookmarkDto.TitleTConst,
            PersonNConst = bookmarkDto.PersonNConst,
            BookmarkType = bookmarkDto.BookmarkType,
            Folder = bookmarkDto.Folder,
            Notes = bookmarkDto.Notes,
            CreatedAt = bookmarkDto.CreatedAt
        };

        return Ok(bookmarkModel);
    }

    [HttpPost(Name = nameof(CreateUserBookmark))]
    public IActionResult CreateUserBookmark(int userId, [FromBody] CreateBookmarkModel createBookmark)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        if (createBookmark == null)
            return BadRequest("Bookmark data is required");

        if (string.IsNullOrEmpty(createBookmark.BookmarkType))
            return BadRequest("BookmarkType is required");

        // Allow either title or person, but not both empty
        if (string.IsNullOrEmpty(createBookmark.TitleTConst) && string.IsNullOrEmpty(createBookmark.PersonNConst))
            return BadRequest("Either TitleTConst or PersonNConst must be provided");

        try
        {
            var bookmark = _dataService.CreateBookmark(
                userId,
                createBookmark.TitleTConst ?? "",
                createBookmark.PersonNConst ?? "",
                createBookmark.BookmarkType,
                createBookmark.Folder ?? "default",
                createBookmark.Notes ?? "");

            var bookmarkModel = new UserBookmarkModel
            {
                Url = GetUrl(nameof(GetUserBookmark), new { userId, bookmarkId = bookmark.BookmarkId }),
                BookmarkId = bookmark.BookmarkId,
                UserId = bookmark.UserId,
                TitleTConst = bookmark.TitleTConst,
                PersonNConst = bookmark.PersonNConst,
                BookmarkType = bookmark.BookmarkType,
                Folder = bookmark.Folder,
                Notes = bookmark.Notes,
                CreatedAt = bookmark.CreatedAt
            };

            return Created(bookmarkModel.Url, bookmarkModel);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating bookmark: {ex.Message}");
        }
    }

    [HttpDelete("{bookmarkId}", Name = nameof(DeleteUserBookmark))]
    public IActionResult DeleteUserBookmark(int userId, int bookmarkId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        if (_dataService.DeleteBookmark(bookmarkId))
        {
            return NoContent();
        }

        return NotFound();
    }
}