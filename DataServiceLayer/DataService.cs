using DataServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataServiceLayer;

public class DataService : IDataService
{
    private readonly string _connectionString;

    public DataService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private ImdbContext CreateContext()
    {
        return new ImdbContext(_connectionString);
    }

    public int GetTitlesCount()
    {
        using var db = CreateContext();
        return db.Titles.Count();
    }

    public IList<Title> GetTitles(int page, int pageSize)
    {
        using var db = CreateContext();
        return db.Titles
            .OrderBy(x => x.TConst)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Title? GetTitle(string tconst)
    {
        using var db = CreateContext();
        return db.Titles.FirstOrDefault(x => x.TConst == tconst);
    }

    public IList<Title> GetTitlesByType(string titleType, int page, int pageSize)
    {
        using var db = CreateContext();
        return db.Titles
            .Where(x => x.TitleType == titleType)
            .OrderBy(x => x.TConst)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int GetPersonsCount()
    {
        using var db = CreateContext();
        return db.Persons.Count();
    }

    public IList<Person> GetPersons(int page, int pageSize)
    {
        using var db = CreateContext();
        return db.Persons
            .OrderBy(x => x.NConst)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Person? GetPerson(string nconst)
    {
        using var db = CreateContext();
        return db.Persons.FirstOrDefault(x => x.NConst == nconst);
    }

    public IList<TitlePerson> GetPersonTitles(string nconst)
    {
        using var db = CreateContext();
        return db.TitlePersons
            .Where(x => x.NConst == nconst)
            .OrderBy(x => x.Ordering)
            .ToList();
    }

    public IList<TitlePerson> GetTitleCast(string tconst)
    {
        using var db = CreateContext();
        return db.TitlePersons
            .Where(x => x.TConst == tconst)
            .OrderBy(x => x.Ordering)
            .ToList();
    }

    public IList<TitleGenre> GetTitleGenres(string tconst)
    {
        using var db = CreateContext();
        return db.TitleGenres
            .Where(x => x.TConst == tconst)
            .ToList();
    }

    public TitleRating? GetTitleRating(string tconst)
    {
        using var db = CreateContext();
        return db.TitleRatings.FirstOrDefault(x => x.TConst == tconst);
    }

    public IList<Title> SimpleSearch(string searchString, int userId)
    {
        using var db = CreateContext();

        if (string.IsNullOrWhiteSpace(searchString))
            return new List<Title>();

        try
        {
            Console.WriteLine($"Searching for: {searchString}");

            var results = db.Titles
                .Where(x => (x.PrimaryTitle != null && x.PrimaryTitle.Contains(searchString)) ||
                           (x.OriginalTitle != null && x.OriginalTitle.Contains(searchString)))
                .Take(50)
                .ToList();

            Console.WriteLine($"Found {results.Count} results");

            if (results.Any())
            {
                LogSearchHistory(userId, searchString, "simple", results.Count);
            }

            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
            return new List<Title>();
        }
    }

    public IList<Title> StructuredSearch(string title, string plot, string characters, string personNames, int userId)
    {
        using var db = CreateContext();

        var query = db.Titles.AsQueryable();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(x => (x.PrimaryTitle != null && x.PrimaryTitle.ToLower().Contains(title.ToLower())) ||
                                    (x.OriginalTitle != null && x.OriginalTitle.ToLower().Contains(title.ToLower())));
        }

        var results = query.Take(50).ToList();

        var searchQuery = $"title:{title}|plot:{plot}|characters:{characters}|persons:{personNames}";
        LogSearchHistory(userId, searchQuery, "structured", results.Count);

        return results;
    }

    public IList<Person> SearchPersons(string searchString)
    {
        using var db = CreateContext();
        return db.Persons
            .Where(x => x.PrimaryName != null && x.PrimaryName.ToLower().Contains(searchString.ToLower()))
            .Take(50)
            .ToList();
    }

    public void LogSearchHistory(int userId, string searchQuery, string searchType, int resultsCount)
    {
        using var db = CreateContext();
        var searchHistory = new SearchHistory
        {
            UserId = userId,
            SearchQuery = searchQuery,
            SearchType = searchType,
            SearchedAt = DateTime.UtcNow
        };

        db.SearchHistories.Add(searchHistory);
        db.SaveChanges();
    }

    public User? GetUser(string username)
    {
        using var db = CreateContext();
        return db.Users.FirstOrDefault(x => x.Username == username);
    }

    public User? GetUser(int userId)
    {
        using var db = CreateContext();
        return db.Users.FirstOrDefault(x => x.UserId == userId);
    }

    public User CreateUser(string username, string email, string passwordHash)
    {
        using var db = CreateContext();
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    public bool ValidateUserCredentials(string username, string passwordHash)
    {
        using var db = CreateContext();
        var user = db.Users.FirstOrDefault(x => x.Username == username);
        return user != null && user.PasswordHash == passwordHash;
    }

    public IList<UserRating> GetUserRatings(int userId)
    {
        using var db = CreateContext();
        return db.UserRatings
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.RatedAt)
            .ToList();
    }

    public UserRating? GetUserRating(int userId, string tconst)
    {
        using var db = CreateContext();
        return db.UserRatings.FirstOrDefault(x => x.UserId == userId && x.TitleTConst == tconst);
    }

    public UserRating CreateUserRating(int userId, string tconst, int rating)
    {
        using var db = CreateContext();

        try
        {
            var existingRating = db.UserRatings
                .FirstOrDefault(x => x.UserId == userId && x.TitleTConst == tconst);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
                existingRating.RatedAt = DateTime.UtcNow;
                db.UserRatings.Update(existingRating);
            }
            else
            
                var userRating = new UserRating
                {
                    UserId = userId,
                    TitleTConst = tconst,
                    Rating = rating,
                    RatedAt = DateTime.UtcNow
                };
                db.UserRatings.Add(userRating);
            }

            db.SaveChanges();
            return existingRating ?? db.UserRatings
                .FirstOrDefault(x => x.UserId == userId && x.TitleTConst == tconst);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Rating creation error: {ex.Message}");
            throw;
        }
    }

    public bool UpdateUserRating(int ratingId, int newRating)
    {
        using var db = CreateContext();
        var rating = db.UserRatings.FirstOrDefault(x => x.RatingId == ratingId);
        if (rating == null) return false;

        rating.Rating = newRating;
        rating.RatedAt = DateTime.UtcNow;
        return db.SaveChanges() > 0;
    }

    public bool DeleteUserRating(int ratingId)
    {
        using var db = CreateContext();
        var rating = db.UserRatings.FirstOrDefault(x => x.RatingId == ratingId);
        if (rating == null) return false;

        db.UserRatings.Remove(rating);
        return db.SaveChanges() > 0;
    }

    public IList<UserBookmark> GetUserBookmarks(int userId)
    {
        using var db = CreateContext();
        return db.UserBookmarks
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public UserBookmark CreateBookmark(int userId, string titleTconst, string personNconst, string bookmarkType, string folder, string notes)
    {
        using var db = CreateContext();

        try
        {
            if (string.IsNullOrEmpty(titleTconst) && string.IsNullOrEmpty(personNconst))
            {
                throw new ArgumentException("Either titleTconst or personNconst must be provided");
            }

            var bookmark = new UserBookmark
            {
                UserId = userId,
                TitleTConst = string.IsNullOrEmpty(titleTconst) ? null : titleTconst.Trim(),
                PersonNConst = string.IsNullOrEmpty(personNconst) ? null : personNconst.Trim(),
                BookmarkType = bookmarkType?.Trim() ?? "general",
                Folder = folder?.Trim() ?? "default",
                Notes = notes?.Trim() ?? "",
                CreatedAt = DateTime.UtcNow
            };

            db.UserBookmarks.Add(bookmark);
            db.SaveChanges();
            return bookmark;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bookmark creation error: {ex.Message}");
            throw;
        }
    }

    public bool DeleteBookmark(int bookmarkId)
    {
        using var db = CreateContext();
        var bookmark = db.UserBookmarks.FirstOrDefault(x => x.BookmarkId == bookmarkId);
        if (bookmark == null) return false;

        db.UserBookmarks.Remove(bookmark);
        return db.SaveChanges() > 0;
    }

    public IList<SearchHistory> GetUserSearchHistory(int userId)
    {
        using var db = CreateContext();
        return db.SearchHistories
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SearchedAt)
            .Take(20)
            .ToList();
    }

    public IList<Profession> GetProfessions()
    {
        using var db = CreateContext();
        return db.Professions.OrderBy(x => x.ProfessionName).ToList();
    }

    public IList<PersonProfession> GetProfessionsByPerson(string nconst)
    {
        using var db = CreateContext();
        return db.PersonProfessions.Where(x => x.NConst == nconst).ToList();
    }

    public IList<TitleAka> GetAkasByTitle(string tconst)
    {
        using var db = CreateContext();
        return db.TitleAkas.Where(x => x.TitleId == tconst).ToList();
    }

    public IList<TitleEpisode> GetEpisodesByTitle(string tconst)
    {
        using var db = CreateContext();
        return db.TitleEpisodes.Where(x => x.ParentTConst == tconst).ToList();
    }

    public TitleEpisode? GetEpisode(string tconst)
    {
        using var db = CreateContext();
        return db.TitleEpisodes.FirstOrDefault(x => x.TConst == tconst);
    }

    public IList<UserNote> GetUserNotes(int userId)
    {
        using var db = CreateContext();
        return db.UserNotes
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToList();
    }

    public UserNote? GetUserNote(int noteId)
    {
        using var db = CreateContext();
        return db.UserNotes.FirstOrDefault(x => x.NoteId == noteId);
    }

    public UserNote CreateUserNote(int userId, string titleTconst, string noteText)
    {
        using var db = CreateContext();
        var note = new UserNote
        {
            UserId = userId,
            TitleTConst = titleTconst,
            NoteText = noteText,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.UserNotes.Add(note);
        db.SaveChanges();
        return note;
    }

    public bool UpdateUserNote(int noteId, string noteText)
    {
        using var db = CreateContext();
        var note = db.UserNotes.FirstOrDefault(x => x.NoteId == noteId);
        if (note == null) return false;

        note.NoteText = noteText;
        note.UpdatedAt = DateTime.UtcNow;
        return db.SaveChanges() > 0;
    }

    public bool DeleteUserNote(int noteId)
    {
        using var db = CreateContext();
        var note = db.UserNotes.FirstOrDefault(x => x.NoteId == noteId);
        if (note == null) return false;

        db.UserNotes.Remove(note);
        return db.SaveChanges() > 0;
    }

    public IList<Genre> GetGenres()
    {
        using var db = CreateContext();
        return db.Genres.OrderBy(x => x.GenreName).ToList();
    }

    public IList<KnownForTitle> GetKnownForTitles(string nconst)
    {
        using var db = CreateContext();
        return db.KnownForTitles.Where(x => x.NConst == nconst).ToList();
    }
}