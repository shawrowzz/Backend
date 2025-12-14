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

    [HttpPost(Name = nameof(CreateUserBookmark))]
    public IActionResult CreateUserBookmark(int userId, [FromBody] CreateBookmarkModel createBookmark)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        try
        {
            if (string.IsNullOrEmpty(createBookmark.BookmarkType))
            {
                return BadRequest(new { message = "Bookmark type is required" });
            }

            if (createBookmark.BookmarkType == "title" && string.IsNullOrEmpty(createBookmark.TitleTConst))
            {
                return BadRequest(new { message = "Title tconst is required for title bookmarks" });
            }

            if (createBookmark.BookmarkType == "person" && string.IsNullOrEmpty(createBookmark.PersonNConst))
            {
                return BadRequest(new { message = "Person nconst is required for person bookmarks" });
            }

            var bookmark = _dataService.CreateBookmark(
                userId,
                createBookmark.TitleTConst,
                createBookmark.PersonNConst,
                createBookmark.BookmarkType,
                createBookmark.Folder ?? "General",
                createBookmark.Notes ?? "");

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
                Url = GetUrl(nameof(GetUserBookmark), new { userId, bookmarkId = bookmarkDto.BookmarkId }),
                BookmarkId = bookmarkDto.BookmarkId,
                UserId = bookmarkDto.UserId,
                TitleTConst = bookmarkDto.TitleTConst,
                PersonNConst = bookmarkDto.PersonNConst,
                BookmarkType = bookmarkDto.BookmarkType,
                Folder = bookmarkDto.Folder,
                Notes = bookmarkDto.Notes,
                CreatedAt = bookmarkDto.CreatedAt
            };

            return Created(bookmarkModel.Url, bookmarkModel);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating bookmark", error = ex.Message });
        }
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