namespace WebServiceLayer.Dto
{
    public class CoPlayerDto
    {
        public string NConst { get; set; }
        public string PrimaryName { get; set; }
        public int CollaborationCount { get; set; }
    }

    public class SimilarMovieDto
    {
        public string TConst { get; set; }
        public string PrimaryTitle { get; set; }
        public decimal SimilarityScore { get; set; }
    }

    public class PersonWordDto
    {
        public string Word { get; set; }
        public int Frequency { get; set; }
    }

    public class ExactMatchResultDto
    {
        public string TConst { get; set; }
        public string PrimaryTitle { get; set; }
        public int MatchCount { get; set; }
    }

    public class BestMatchResultDto
    {
        public string TConst { get; set; }
        public string PrimaryTitle { get; set; }
        public int MatchCount { get; set; }
    }

    public class PopularActorDto
    {
        public string NConst { get; set; }
        public string PrimaryName { get; set; }
        public string Category { get; set; }
        public string CharacterName { get; set; }
        public decimal? PopularityRank { get; set; }
    }
}