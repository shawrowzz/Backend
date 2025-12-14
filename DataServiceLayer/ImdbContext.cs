using DataServiceLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class ImdbContext : DbContext
{
    private readonly string _connectionString;

    public ImdbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ImdbContext(DbContextOptions<ImdbContext> options) : base(options)
    {
    }
    public DbSet<OmdbData> OmdbDatas { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<TitlePerson> TitlePersons { get; set; }
    public DbSet<TitleGenre> TitleGenres { get; set; }
    public DbSet<TitleRating> TitleRatings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRating> UserRatings { get; set; }
    public DbSet<UserBookmark> UserBookmarks { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }
    public DbSet<Profession> Professions { get; set; }
    public DbSet<PersonProfession> PersonProfessions { get; set; }
    public DbSet<TitleAka> TitleAkas { get; set; }
    public DbSet<TitleEpisode> TitleEpisodes { get; set; }
    public DbSet<UserNote> UserNotes { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<KnownForTitle> KnownForTitles { get; set; }
    public DbSet<WiWord> WiWords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OmdbData>().ToTable("omdb_datas");
        modelBuilder.Entity<Title>().ToTable("title");
        modelBuilder.Entity<Person>().ToTable("person");
        modelBuilder.Entity<TitlePerson>().ToTable("title_person");
        modelBuilder.Entity<TitleGenre>().ToTable("title_genre");
        modelBuilder.Entity<TitleRating>().ToTable("title_rating");
        modelBuilder.Entity<Profession>().ToTable("profession");
        modelBuilder.Entity<PersonProfession>().ToTable("person_profession");
        modelBuilder.Entity<TitleAka>().ToTable("title_aka");
        modelBuilder.Entity<TitleEpisode>().ToTable("title_episodes");
        modelBuilder.Entity<Genre>().ToTable("genre");
        modelBuilder.Entity<KnownForTitle>().ToTable("known_for_title");
        modelBuilder.Entity<WiWord>().ToTable("wi");

        modelBuilder.Entity<User>().ToTable("app_user");
        modelBuilder.Entity<UserRating>().ToTable("user_rating");
        modelBuilder.Entity<UserBookmark>().ToTable("user_bookmark");
        modelBuilder.Entity<UserNote>().ToTable("user_note");
        modelBuilder.Entity<SearchHistory>().ToTable("search_history");


        ConfigureOmdbData(modelBuilder);
        ConfigureTitle(modelBuilder);
        ConfigurePerson(modelBuilder);
        ConfigureTitlePerson(modelBuilder);
        ConfigureTitleGenre(modelBuilder);
        ConfigureTitleRating(modelBuilder);
        ConfigureProfession(modelBuilder);
        ConfigurePersonProfession(modelBuilder);
        ConfigureTitleAka(modelBuilder);
        ConfigureTitleEpisode(modelBuilder);
        ConfigureGenre(modelBuilder);
        ConfigureKnownForTitle(modelBuilder);
        ConfigureWiWord(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureUserRating(modelBuilder);
        ConfigureUserBookmark(modelBuilder);
        ConfigureUserNote(modelBuilder);
        ConfigureSearchHistory(modelBuilder);
    }

    private void ConfigureOmdbData(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<OmdbData>().HasKey(x => x.TConst);
        modelBuilder.Entity<OmdbData>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<OmdbData>().Property(x => x.Episode).HasColumnName("episode");
        modelBuilder.Entity<OmdbData>().Property(x => x.Awards).HasColumnName("awards");
        modelBuilder.Entity<OmdbData>().Property(x => x.Plot).HasColumnName("plot");
        modelBuilder.Entity<OmdbData>().Property(x => x.SeriesId).HasColumnName("seriesid");
        modelBuilder.Entity<OmdbData>().Property(x => x.Rated).HasColumnName("rated");
        modelBuilder.Entity<OmdbData>().Property(x => x.ImdbRating).HasColumnName("imdbrating");
        modelBuilder.Entity<OmdbData>().Property(x => x.Runtime).HasColumnName("runtime");
        modelBuilder.Entity<OmdbData>().Property(x => x.Language).HasColumnName("language");
        modelBuilder.Entity<OmdbData>().Property(x => x.Released).HasColumnName("released");
        modelBuilder.Entity<OmdbData>().Property(x => x.Response).HasColumnName("response");
        modelBuilder.Entity<OmdbData>().Property(x => x.Writer).HasColumnName("writer");
        modelBuilder.Entity<OmdbData>().Property(x => x.Genre).HasColumnName("genre");
        modelBuilder.Entity<OmdbData>().Property(x => x.Title).HasColumnName("title");
        modelBuilder.Entity<OmdbData>().Property(x => x.Country).HasColumnName("country");
        modelBuilder.Entity<OmdbData>().Property(x => x.Dvd).HasColumnName("dvd");
        modelBuilder.Entity<OmdbData>().Property(x => x.Production).HasColumnName("production");
        modelBuilder.Entity<OmdbData>().Property(x => x.Season).HasColumnName("season");
        modelBuilder.Entity<OmdbData>().Property(x => x.Type).HasColumnName("type");
        modelBuilder.Entity<OmdbData>().Property(x => x.Poster).HasColumnName("poster");
    }

    private void ConfigureTitle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Title>().HasKey(x => x.TConst);
        modelBuilder.Entity<Title>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<Title>().Property(x => x.TitleType).HasColumnName("titletype");
        modelBuilder.Entity<Title>().Property(x => x.PrimaryTitle).HasColumnName("primarytitle");
        modelBuilder.Entity<Title>().Property(x => x.OriginalTitle).HasColumnName("originaltitle");
        modelBuilder.Entity<Title>().Property(x => x.IsAdult).HasColumnName("isadult");
        modelBuilder.Entity<Title>().Property(x => x.StartYear).HasColumnName("startyear");
        modelBuilder.Entity<Title>().Property(x => x.EndYear).HasColumnName("endyear");
        modelBuilder.Entity<Title>().Property(x => x.RuntimeMinutes).HasColumnName("runtimeminutes");
    }

    private void ConfigurePerson(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasKey(x => x.NConst);
        modelBuilder.Entity<Person>().Property(x => x.NConst).HasColumnName("nconst");
        modelBuilder.Entity<Person>().Property(x => x.PrimaryName).HasColumnName("primaryname");
        modelBuilder.Entity<Person>().Property(x => x.BirthYear).HasColumnName("birthyear");
        modelBuilder.Entity<Person>().Property(x => x.DeathYear).HasColumnName("deathyear");
        modelBuilder.Entity<Person>().Property(x => x.WeightedRating).HasColumnName("weighted_rating");
        modelBuilder.Entity<Person>().Property(x => x.RatingWeight).HasColumnName("rating_weight");
    }

    private void ConfigureTitlePerson(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitlePerson>().HasKey(x => new { x.TConst, x.NConst, x.Ordering });
        modelBuilder.Entity<TitlePerson>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<TitlePerson>().Property(x => x.NConst).HasColumnName("nconst");
        modelBuilder.Entity<TitlePerson>().Property(x => x.Ordering).HasColumnName("ordering");
        modelBuilder.Entity<TitlePerson>().Property(x => x.Category).HasColumnName("category");
        modelBuilder.Entity<TitlePerson>().Property(x => x.Job).HasColumnName("job");
        modelBuilder.Entity<TitlePerson>().Property(x => x.CharacterName).HasColumnName("charactername");
    }

    private void ConfigureTitleGenre(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitleGenre>().HasKey(x => new { x.TConst, x.GenreName });
        modelBuilder.Entity<TitleGenre>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<TitleGenre>().Property(x => x.GenreName).HasColumnName("genre_name");
    }

    private void ConfigureTitleRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitleRating>().HasKey(x => x.TConst);
        modelBuilder.Entity<TitleRating>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<TitleRating>().Property(x => x.AverageRating).HasColumnName("averagerating");
        modelBuilder.Entity<TitleRating>().Property(x => x.NumVotes).HasColumnName("numvotes");
    }

    private void ConfigureProfession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profession>().HasKey(x => x.ProfessionName);
        modelBuilder.Entity<Profession>().Property(x => x.ProfessionName).HasColumnName("profession_name");
    }

    private void ConfigurePersonProfession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonProfession>().HasKey(x => new { x.NConst, x.ProfessionName });
        modelBuilder.Entity<PersonProfession>().Property(x => x.NConst).HasColumnName("nconst");
        modelBuilder.Entity<PersonProfession>().Property(x => x.ProfessionName).HasColumnName("profession_name");
    }

    private void ConfigureTitleAka(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitleAka>().HasKey(x => new { x.TitleId, x.Ordering });
        modelBuilder.Entity<TitleAka>().Property(x => x.TitleId).HasColumnName("titleid");
        modelBuilder.Entity<TitleAka>().Property(x => x.Ordering).HasColumnName("ordering");
        modelBuilder.Entity<TitleAka>().Property(x => x.Title).HasColumnName("title");
        modelBuilder.Entity<TitleAka>().Property(x => x.Region).HasColumnName("region");
        modelBuilder.Entity<TitleAka>().Property(x => x.Language).HasColumnName("language");
        modelBuilder.Entity<TitleAka>().Property(x => x.Types).HasColumnName("types");
        modelBuilder.Entity<TitleAka>().Property(x => x.Attributes).HasColumnName("attributes");
        modelBuilder.Entity<TitleAka>().Property(x => x.IsOriginalTitle).HasColumnName("isoriginaltitle");
    }

    private void ConfigureTitleEpisode(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitleEpisode>().HasKey(x => x.TConst);
        modelBuilder.Entity<TitleEpisode>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<TitleEpisode>().Property(x => x.ParentTConst).HasColumnName("parenttconst");
        modelBuilder.Entity<TitleEpisode>().Property(x => x.SeasonNumber).HasColumnName("seasonnumber");
        modelBuilder.Entity<TitleEpisode>().Property(x => x.EpisodeNumber).HasColumnName("episodenumber");
    }

    private void ConfigureGenre(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasKey(x => x.GenreName);
        modelBuilder.Entity<Genre>().Property(x => x.GenreName).HasColumnName("genre_name");
    }

    private void ConfigureKnownForTitle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnownForTitle>().HasKey(x => new { x.NConst, x.TConst });
        modelBuilder.Entity<KnownForTitle>().Property(x => x.NConst).HasColumnName("nconst");
        modelBuilder.Entity<KnownForTitle>().Property(x => x.TConst).HasColumnName("tconst");
    }

    private void ConfigureWiWord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WiWord>().HasNoKey();
        modelBuilder.Entity<WiWord>().Property(x => x.TConst).HasColumnName("tconst");
        modelBuilder.Entity<WiWord>().Property(x => x.Word).HasColumnName("word");
        modelBuilder.Entity<WiWord>().Property(x => x.Field).HasColumnName("field");
        modelBuilder.Entity<WiWord>().Property(x => x.Lexme).HasColumnName("lexme");
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.UserId);
        modelBuilder.Entity<User>().Property(x => x.UserId).HasColumnName("user_id");
        modelBuilder.Entity<User>().Property(x => x.Username).HasColumnName("username");
        modelBuilder.Entity<User>().Property(x => x.Email).HasColumnName("email");
        modelBuilder.Entity<User>().Property(x => x.PasswordHash).HasColumnName("password_hash");
        modelBuilder.Entity<User>().Property(x => x.CreatedAt).HasColumnName("created_at");
    }

    private void ConfigureUserRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRating>().HasKey(x => x.RatingId);
        modelBuilder.Entity<UserRating>().Property(x => x.RatingId).HasColumnName("rating_id");
        modelBuilder.Entity<UserRating>().Property(x => x.UserId).HasColumnName("user_id");
        modelBuilder.Entity<UserRating>().Property(x => x.TitleTConst).HasColumnName("title_tconst");
        modelBuilder.Entity<UserRating>().Property(x => x.Rating).HasColumnName("rating");
        modelBuilder.Entity<UserRating>().Property(x => x.RatedAt).HasColumnName("rated_at");
    }

    private void ConfigureUserBookmark(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBookmark>().HasKey(x => x.BookmarkId);
        modelBuilder.Entity<UserBookmark>().Property(x => x.BookmarkId).HasColumnName("bookmark_id");
        modelBuilder.Entity<UserBookmark>().Property(x => x.UserId).HasColumnName("user_id");
        modelBuilder.Entity<UserBookmark>().Property(x => x.TitleTConst).HasColumnName("title_tconst");
        modelBuilder.Entity<UserBookmark>().Property(x => x.PersonNConst).HasColumnName("person_nconst");
        modelBuilder.Entity<UserBookmark>().Property(x => x.BookmarkType).HasColumnName("bookmark_type");
        modelBuilder.Entity<UserBookmark>().Property(x => x.Folder).HasColumnName("folder");
        modelBuilder.Entity<UserBookmark>().Property(x => x.Notes).HasColumnName("notes");
        modelBuilder.Entity<UserBookmark>().Property(x => x.CreatedAt).HasColumnName("created_at");
    }

    private void ConfigureUserNote(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNote>().HasKey(x => x.NoteId);
        modelBuilder.Entity<UserNote>().Property(x => x.NoteId).HasColumnName("note_id");
        modelBuilder.Entity<UserNote>().Property(x => x.UserId).HasColumnName("user_id");
        modelBuilder.Entity<UserNote>().Property(x => x.TitleTConst).HasColumnName("title_tconst");
        modelBuilder.Entity<UserNote>().Property(x => x.NoteText).HasColumnName("note_text");
        modelBuilder.Entity<UserNote>().Property(x => x.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<UserNote>().Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }

    private void ConfigureSearchHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SearchHistory>().HasKey(x => x.SearchId);
        modelBuilder.Entity<SearchHistory>().Property(x => x.SearchId).HasColumnName("search_id");
        modelBuilder.Entity<SearchHistory>().Property(x => x.UserId).HasColumnName("user_id");
        modelBuilder.Entity<SearchHistory>().Property(x => x.SearchQuery).HasColumnName("search_query");
        modelBuilder.Entity<SearchHistory>().Property(x => x.SearchType).HasColumnName("search_type");
        modelBuilder.Entity<SearchHistory>().Property(x => x.SearchResultsCount).HasColumnName("search_results_count");
        modelBuilder.Entity<SearchHistory>().Property(x => x.SearchedAt).HasColumnName("searched_at");
    }
}