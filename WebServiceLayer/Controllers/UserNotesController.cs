using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Dto;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users/{userId}/notes")]
public class UserNotesController : BaseController
{
    public UserNotesController(IDataService dataService, LinkGenerator generator)
        : base(dataService, generator)
    {
    }

    [HttpGet(Name = nameof(GetUserNotes))]
    public IActionResult GetUserNotes(int userId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var notes = _dataService.GetUserNotes(userId)
            .Select(x => new UserNoteDto
            {
                NoteId = x.NoteId,
                UserId = x.UserId,
                TitleTConst = x.TitleTConst,
                NoteText = x.NoteText,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .Select(dto => new UserNoteModel
            {
                Url = GetUrl(nameof(GetUserNote), new { userId, noteId = dto.NoteId }),
                NoteId = dto.NoteId,
                UserId = dto.UserId,
                TitleTConst = dto.TitleTConst,
                NoteText = dto.NoteText,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            })
            .ToList();

        return Ok(notes);
    }

    [HttpGet("{noteId}", Name = nameof(GetUserNote))]
    public IActionResult GetUserNote(int userId, int noteId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var note = _dataService.GetUserNote(noteId);
        if (note == null || note.UserId != userId) return NotFound();

        var noteDto = new UserNoteDto
        {
            NoteId = note.NoteId,
            UserId = note.UserId,
            TitleTConst = note.TitleTConst,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };

        var noteModel = new UserNoteModel
        {
            Url = GetUrl(nameof(GetUserNote), new { userId, noteId }),
            NoteId = noteDto.NoteId,
            UserId = noteDto.UserId,
            TitleTConst = noteDto.TitleTConst,
            NoteText = noteDto.NoteText,
            CreatedAt = noteDto.CreatedAt,
            UpdatedAt = noteDto.UpdatedAt
        };

        return Ok(noteModel);
    }

    [HttpPost(Name = nameof(CreateUserNote))]
    public IActionResult CreateUserNote(int userId, [FromBody] CreateNoteModel createNote)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        var note = _dataService.CreateUserNote(userId, createNote.TitleTConst, createNote.NoteText);

        var noteDto = new UserNoteDto
        {
            NoteId = note.NoteId,
            UserId = note.UserId,
            TitleTConst = note.TitleTConst,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };

        var noteModel = new UserNoteModel
        {
            Url = GetUrl(nameof(GetUserNote), new { userId, noteId = noteDto.NoteId }),
            NoteId = noteDto.NoteId,
            UserId = noteDto.UserId,
            TitleTConst = noteDto.TitleTConst,
            NoteText = noteDto.NoteText,
            CreatedAt = noteDto.CreatedAt,
            UpdatedAt = noteDto.UpdatedAt
        };

        return Created(noteModel.Url, noteModel);
    }

    [HttpPut("{noteId}", Name = nameof(UpdateUserNote))]
    public IActionResult UpdateUserNote(int userId, int noteId, [FromBody] CreateNoteModel updateNote)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        if (_dataService.UpdateUserNote(noteId, updateNote.NoteText))
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpDelete("{noteId}", Name = nameof(DeleteUserNote))]
    public IActionResult DeleteUserNote(int userId, int noteId)
    {
        var authResult = RequireUserMatch(userId);
        if (authResult != null) return authResult;

        if (_dataService.DeleteUserNote(noteId))
        {
            return NoContent();
        }

        return NotFound();
    }
}