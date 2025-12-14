using System;
using DataServiceLayer.Models;
using System.Collections.Generic;
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

    public OmdbData? GetOmdbData(string tconst)
    {
        using var db = CreateContext();
        return db.OmdbDatas.FirstOrDefault(x => x.TConst == tconst);
    }

    public IList<Title> StructuredSearch(string titleQuery, string plot, string characters, string personNames, int userId)
    {
        using var db = CreateContext();

        // Start with ALL titles
        var allTitlesQuery = db.Titles.AsQueryable();
        var filteredTitles = new List<string>();
        bool hasFilter = false;

        
        if (!string.IsNullOrWhiteSpace(titleQuery))
        {
            titleQuery = titleQuery.ToLower();
            var titleMatches = allTitlesQuery
                .Where(x => (x.PrimaryTitle != null && x.PrimaryTitle.ToLower().Contains(titleQuery)) ||
                           (x.OriginalTitle != null && x.OriginalTitle.ToLower().Contains(titleQuery)))
                .Select(x => x.TConst)
                .Distinct()
                .ToList();

            if (titleMatches.Any())
            {
                filteredTitles = titleMatches;
                hasFilter = true;
            }
            else
            {
                // No title matches, return empty
                LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                               "structured_string", 0);
                return new List<Title>();
            }
        }

        
        if (!string.IsNullOrWhiteSpace(plot))
        {
            plot = plot.ToLower();
            var plotMatches = db.OmdbDatas
                .Where(o => o.Plot != null && o.Plot.ToLower().Contains(plot))
                .Select(o => o.TConst)
                .Distinct()
                .ToList();

            if (plotMatches.Any())
            {
                // If we already have filtered titles, intersect with plot matches
                if (filteredTitles.Any())
                {
                    filteredTitles = filteredTitles.Intersect(plotMatches).ToList();
                }
                else
                {
                    filteredTitles = plotMatches;
                }
                hasFilter = true;
            }
            else
            {
                // No plot matches, return empty
                LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                               "structured_string", 0);
                return new List<Title>();
            }
        }

        
        if (!string.IsNullOrWhiteSpace(characters))
        {
            characters = characters.ToLower();
            var characterMatches = db.TitlePersons
                .Where(tp => tp.CharacterName != null && tp.CharacterName.ToLower().Contains(characters))
                .Select(tp => tp.TConst)
                .Distinct()
                .ToList();

            if (characterMatches.Any())
            {
                if (filteredTitles.Any())
                {
                    filteredTitles = filteredTitles.Intersect(characterMatches).ToList();
                }
                else
                {
                    filteredTitles = characterMatches;
                }
                hasFilter = true;
            }
            else
            {
                LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                               "structured_string", 0);
                return new List<Title>();
            }
        }

        
        if (!string.IsNullOrWhiteSpace(personNames))
        {
            personNames = personNames.ToLower();
            var personMatches = db.TitlePersons
                .Join(db.Persons,
                    tp => tp.NConst,
                    p => p.NConst,
                    (tp, p) => new { tp.TConst, p.PrimaryName })
                .Where(x => x.PrimaryName != null && x.PrimaryName.ToLower().Contains(personNames))
                .Select(x => x.TConst)
                .Distinct()
                .ToList();

            if (personMatches.Any())
            {
                if (filteredTitles.Any())
                {
                    filteredTitles = filteredTitles.Intersect(personMatches).ToList();
                }
                else
                {
                    filteredTitles = personMatches;
                }
                hasFilter = true;
            }
            else
            {
                LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                               "structured_string", 0);
                return new List<Title>();
            }
        }

        
        if (!hasFilter)
        {
            LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                           "structured_string", 0);
            return new List<Title>();
        }

        // Get the actual title objects
        var results = db.Titles
            .Where(t => filteredTitles.Contains(t.TConst))
            .OrderByDescending(x => x.StartYear)
            .Take(50)
            .ToList();

        // Log search history
        LogSearchHistory(userId, $"title:{titleQuery}|plot:{plot}|characters:{characters}|persons:{personNames}",
                       "structured_string", results.Count);

        return results;
    }

    public IList<Title> SimpleSearch(string searchString, int userId)
    {
        using var db = CreateContext();

        if (string.IsNullOrWhiteSpace(searchString))
            return new List<Title>();

        try
        {
            var results = db.Titles
                .Where(x => (x.PrimaryTitle != null && x.PrimaryTitle.ToLower().Contains(searchString.ToLower())) ||
                           (x.OriginalTitle != null && x.OriginalTitle.ToLower().Contains(searchString.ToLower())))
                .OrderByDescending(x => x.StartYear)
                .Take(50)
                .ToList();

            // Log search history
            if (results.Any())
            {
                try
                {
                    LogSearchHistory(userId, searchString, "simple_string", results.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to log search history: {ex.Message}");
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Simple search error: {ex.Message}");
            return new List<Title>();
        }
    }

    public void LogSearchHistory(int userId, string searchQuery, string searchType, int resultsCount)
    {
        using var db = CreateContext();

        // Ensure search query doesn't exceed database constraints
        if (searchQuery.Length > 500)
        {
            searchQuery = searchQuery.Substring(0, 500);
        }

        // Ensure search type is within 20 characters (varchar(20) constraint)
        if (searchType.Length > 20)
        {
            searchType = searchType.Substring(0, 20);
        }

        var searchHistory = new SearchHistory
        {
            UserId = userId,
            SearchQuery = searchQuery,
            SearchType = searchType,
            SearchResultsCount = resultsCount,
            SearchedAt = DateTime.UtcNow
        };

        try
        {
            db.SearchHistories.Add(searchHistory);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the search
            Console.WriteLine($"Failed to log search history: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
        }
    }

    public IList<SearchHistory> GetUserSearchHistory(int userId)
    {
        using var db = CreateContext();

        // Simple LINQ query - now EF should know about SearchResultsCount
        return db.SearchHistories
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SearchedAt)
            .Take(20)
            .ToList();
    }

    public bool ClearSearchHistory(int userId)
    {
        using var db = CreateContext();

        var deleted = db.SearchHistories
            .Where(x => x.UserId == userId)
            .ExecuteDelete();

        return deleted > 0;
    }

    public UserBookmark CreateBookmark(int userId, string titleTconst, string personNconst, string bookmarkType, string folder, string notes)
    {
        using var db = CreateContext();

        // Check for duplicate
        UserBookmark? existingBookmark = null;

        if (bookmarkType == "title" && !string.IsNullOrEmpty(titleTconst))
        {
            existingBookmark = db.UserBookmarks
                .FirstOrDefault(x => x.UserId == userId && x.TitleTConst == titleTconst);
        }
        else if (bookmarkType == "person" && !string.IsNullOrEmpty(personNconst))
        {
            existingBookmark = db.UserBookmarks
                .FirstOrDefault(x => x.UserId == userId && x.PersonNConst == personNconst);
        }

        if (existingBookmark != null)
        {
            throw new InvalidOperationException("Bookmark already exists");
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

    public UserNote CreateUserNote(int userId, string titleTconst, string noteText)
    {
        using var db = CreateContext();

        // Check for existing note
        var existingNote = db.UserNotes
            .FirstOrDefault(x => x.UserId == userId && x.TitleTConst == titleTconst);

        if (existingNote != null)
        {
            throw new InvalidOperationException("Note already exists for this title");
        }

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

    public IList<Person> SearchPersons(string searchString)
    {
        using var db = CreateContext();
        return db.Persons
            .Where(x => x.PrimaryName != null && x.PrimaryName.ToLower().Contains(searchString.ToLower()))
            .Take(50)
            .ToList();
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
        var existingRating = db.UserRatings
            .FirstOrDefault(x => x.UserId == userId && x.TitleTConst == tconst);

        if (existingRating != null)
        {
            throw new InvalidOperationException("You have already rated this title");
        }

        var userRating = new UserRating
        {
            UserId = userId,
            TitleTConst = tconst,
            Rating = rating,
            RatedAt = DateTime.UtcNow
        };

        db.UserRatings.Add(userRating);
        db.SaveChanges();

        return userRating;
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

    public bool DeleteBookmark(int bookmarkId)
    {
        using var db = CreateContext();
        var bookmark = db.UserBookmarks.FirstOrDefault(x => x.BookmarkId == bookmarkId);
        if (bookmark == null) return false;

        db.UserBookmarks.Remove(bookmark);
        return db.SaveChanges() > 0;
    }

    public IList<Profession> GetProfessions()
    {
        using var db = CreateContext();
        return db.Professions
            .OrderBy(x => x.ProfessionName)
            .ToList();
    }

    public IList<PersonProfession> GetProfessionsByPerson(string nconst)
    {
        using var db = CreateContext();
        return db.PersonProfessions
            .Where(x => x.NConst == nconst)
            .ToList();
    }

    public IList<TitleAka> GetAkasByTitle(string tconst)
    {
        using var db = CreateContext();
        return db.TitleAkas
            .Where(x => x.TitleId == tconst)
            .ToList();
    }

    public IList<TitleEpisode> GetEpisodesByTitle(string tconst)
    {
        using var db = CreateContext();
        return db.TitleEpisodes
            .Where(x => x.ParentTConst == tconst)
            .ToList();
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
        return db.Genres
            .OrderBy(x => x.GenreName)
            .ToList();
    }

    public IList<KnownForTitle> GetKnownForTitles(string nconst)
    {
        using var db = CreateContext();
        return db.KnownForTitles
            .Where(x => x.NConst == nconst)
            .ToList();
    }

    public List<CoPlayer> FindCoPlayers(string actorName)
    {
        using var db = CreateContext();

        // Find the actor's movies
        var actorMovies = db.TitlePersons
            .Where(tp => tp.Category == "actor" || tp.Category == "actress")
            .Join(db.Persons.Where(p => p.PrimaryName.Contains(actorName)),
                tp => tp.NConst,
                p => p.NConst,
                (tp, p) => tp.TConst)
            .Distinct()
            .ToList();

        if (!actorMovies.Any())
            return new List<CoPlayer>();

        // Find co-players in those movies
        var coPlayers = db.TitlePersons
            .Where(tp => actorMovies.Contains(tp.TConst) &&
                        (tp.Category == "actor" || tp.Category == "actress"))
            .Join(db.Persons,
                tp => tp.NConst,
                p => p.NConst,
                (tp, p) => new { tp.NConst, p.PrimaryName })
            .Where(x => !x.PrimaryName.Contains(actorName)) // Exclude the original actor
            .GroupBy(x => new { x.NConst, x.PrimaryName })
            .Select(g => new CoPlayer
            {
                NConst = g.Key.NConst,
                PrimaryName = g.Key.PrimaryName,
                CollaborationCount = g.Count()
            })
            .OrderByDescending(cp => cp.CollaborationCount)
            .Take(10)
            .ToList();

        return coPlayers;
    }

    public List<SimilarMovie> FindSimilarMovies(string tconst, int limit = 10)
    {
        using var db = CreateContext();

        var movie = db.Titles.FirstOrDefault(t => t.TConst == tconst);
        if (movie == null)
            return new List<SimilarMovie>();

        // Get movie genres
        var movieGenres = db.TitleGenres
            .Where(tg => tg.TConst == tconst)
            .Select(tg => tg.GenreName)
            .ToList();

        // Get movie directors
        var movieDirectors = db.TitlePersons
            .Where(tp => tp.TConst == tconst && tp.Category == "director")
            .Select(tp => tp.NConst)
            .ToList();

        // Find similar movies based on genres
        var similarMovies = db.TitleGenres
            .Where(tg => movieGenres.Contains(tg.GenreName) && tg.TConst != tconst)
            .GroupBy(tg => tg.TConst)
            .Select(g => new
            {
                TConst = g.Key,
                GenreScore = g.Count() * 2.0m,
                DirectorScore = db.TitlePersons
                    .Count(tp => tp.TConst == g.Key &&
                                tp.Category == "director" &&
                                movieDirectors.Contains(tp.NConst)) * 1.5m
            })
            .ToList();

        // Join with titles and ratings
        var result = similarMovies
            .Join(db.Titles,
                sm => sm.TConst,
                t => t.TConst,
                (sm, t) => new { sm.TConst, sm.GenreScore, sm.DirectorScore, PrimaryTitle = t.PrimaryTitle ?? "" })
            .GroupJoin(db.TitleRatings,
                x => x.TConst,
                tr => tr.TConst,
                (x, ratings) => new
                {
                    x.TConst,
                    x.PrimaryTitle,
                    x.GenreScore,
                    x.DirectorScore,
                    RatingScore = ratings.FirstOrDefault() != null ?
                                 ratings.FirstOrDefault().AverageRating / 10.0m : 0
                })
            .Select(x => new SimilarMovie
            {
                TConst = x.TConst,
                PrimaryTitle = x.PrimaryTitle,
                SimilarityScore = x.GenreScore + x.DirectorScore + x.RatingScore
            })
            .OrderByDescending(m => m.SimilarityScore)
            .Take(limit)
            .ToList();

        return result;
    }

    public List<PersonWord> GetPersonWords(string personName, int limit = 10)
    {
        using var db = CreateContext();

        // Find person's titles
        var personTitles = db.TitlePersons
            .Join(db.Persons.Where(p => p.PrimaryName.Contains(personName)),
                tp => tp.NConst,
                p => p.NConst,
                (tp, p) => tp.TConst)
            .Distinct()
            .ToList();

        if (!personTitles.Any())
            return new List<PersonWord>();

        // Get words from wi table for these titles
        var personWords = db.WiWords
            .Where(w => personTitles.Contains(w.TConst))
            .GroupBy(w => w.Word)
            .Select(g => new PersonWord
            {
                Word = g.Key,
                Frequency = g.Count()
            })
            .OrderByDescending(pw => pw.Frequency)
            .Take(limit)
            .ToList();

        return personWords;
    }

    public List<ExactMatchResult> ExactMatchSearch(string[] keywords, int limit = 50)
    {
        using var db = CreateContext();

        var results = db.WiWords
            .Where(w => keywords.Contains(w.Word))
            .GroupBy(w => w.TConst)
            .Select(g => new
            {
                TConst = g.Key,
                MatchCount = g.Select(w => w.Word).Distinct().Count()
            })
            .Where(x => x.MatchCount == keywords.Length) // All keywords must match
            .Join(db.Titles,
                x => x.TConst,
                t => t.TConst,
                (x, t) => new ExactMatchResult
                {
                    TConst = x.TConst,
                    PrimaryTitle = t.PrimaryTitle ?? "",
                    MatchCount = x.MatchCount
                })
            .OrderByDescending(r => r.MatchCount)
            .ThenBy(r => r.PrimaryTitle)
            .Take(limit)
            .ToList();

        return results;
    }

    public List<BestMatchResult> BestMatchSearch(string[] keywords, int limit = 50)
    {
        using var db = CreateContext();

        var results = db.WiWords
            .Where(w => keywords.Contains(w.Word))
            .GroupBy(w => w.TConst)
            .Select(g => new
            {
                TConst = g.Key,
                MatchCount = g.Select(w => w.Word).Distinct().Count()
            })
            .Join(db.Titles,
                x => x.TConst,
                t => t.TConst,
                (x, t) => new BestMatchResult
                {
                    TConst = x.TConst,
                    PrimaryTitle = t.PrimaryTitle ?? "",
                    MatchCount = x.MatchCount
                })
            .OrderByDescending(r => r.MatchCount)
            .ThenBy(r => r.PrimaryTitle)
            .Take(limit)
            .ToList();

        return results;
    }

    public decimal? GetAverageRating(string tconst)
    {
        using var db = CreateContext();

        var rating = db.TitleRatings
            .FirstOrDefault(tr => tr.TConst == tconst);

        return rating?.AverageRating;
    }

    public List<PopularActor> GetPopularActorsInMovie(string tconst)
    {
        using var db = CreateContext();

        var popularActors = db.TitlePersons
            .Where(tp => tp.TConst == tconst &&
                        (tp.Category == "actor" || tp.Category == "actress"))
            .Join(db.Persons,
                tp => tp.NConst,
                p => p.NConst,
                (tp, p) => new PopularActor
                {
                    NConst = tp.NConst,
                    PrimaryName = p.PrimaryName ?? "",
                    Category = tp.Category ?? "",
                    CharacterName = tp.CharacterName ?? "",
                    PopularityRank = p.WeightedRating
                })
            .OrderByDescending(pa => pa.PopularityRank)
            .ThenBy(pa => pa.PrimaryName)
            .ToList();

        return popularActors;
    }
}