using System.Collections.Generic;
using DataServiceLayer.Models;

namespace DataServiceLayer;

public interface IDataService
{
    int GetTitlesCount();
    IList<Title> GetTitles(int page, int pageSize);
    Title? GetTitle(string tconst);
    IList<Title> GetTitlesByType(string titleType, int page, int pageSize);

    int GetPersonsCount();
    IList<Person> GetPersons(int page, int pageSize);
    Person? GetPerson(string nconst);
    IList<TitlePerson> GetPersonTitles(string nconst);

    IList<TitlePerson> GetTitleCast(string tconst);
    IList<TitleGenre> GetTitleGenres(string tconst);
    TitleRating? GetTitleRating(string tconst);

    IList<Title> SimpleSearch(string searchString, int userId);
    IList<Title> StructuredSearch(string title, string plot, string characters, string personNames, int userId);
    IList<Person> SearchPersons(string searchString);
    void LogSearchHistory(int userId, string searchQuery, string searchType, int resultsCount);

    User? GetUser(string username);
    User? GetUser(int userId);
    User CreateUser(string username, string email, string passwordHash);
    bool ValidateUserCredentials(string username, string passwordHash);

    IList<UserRating> GetUserRatings(int userId);
    UserRating? GetUserRating(int userId, string tconst);
    UserRating CreateUserRating(int userId, string tconst, int rating);
    bool UpdateUserRating(int ratingId, int newRating);
    bool DeleteUserRating(int ratingId);

    IList<UserBookmark> GetUserBookmarks(int userId);
    UserBookmark CreateBookmark(int userId, string titleTconst, string personNconst, string bookmarkType, string folder, string notes);
    bool DeleteBookmark(int bookmarkId);

    IList<SearchHistory> GetUserSearchHistory(int userId);
    bool ClearSearchHistory(int userId); 

    IList<Profession> GetProfessions();
    IList<PersonProfession> GetProfessionsByPerson(string nconst);

    IList<TitleAka> GetAkasByTitle(string tconst);

    IList<TitleEpisode> GetEpisodesByTitle(string tconst);
    TitleEpisode? GetEpisode(string tconst);

    IList<UserNote> GetUserNotes(int userId);
    UserNote? GetUserNote(int noteId);
    UserNote CreateUserNote(int userId, string titleTconst, string noteText);
    bool UpdateUserNote(int noteId, string noteText);
    bool DeleteUserNote(int noteId);

    IList<Genre> GetGenres();

    IList<KnownForTitle> GetKnownForTitles(string nconst);

    List<CoPlayer> FindCoPlayers(string actorName);

    List<SimilarMovie> FindSimilarMovies(string tconst, int limit = 10);

    List<PersonWord> GetPersonWords(string personName, int limit = 10);

    List<ExactMatchResult> ExactMatchSearch(string[] keywords, int limit = 50);

    List<BestMatchResult> BestMatchSearch(string[] keywords, int limit = 50);
    decimal? GetAverageRating(string tconst);

    List<PopularActor> GetPopularActorsInMovie(string tconst);

    OmdbData? GetOmdbData(string tconst);
}