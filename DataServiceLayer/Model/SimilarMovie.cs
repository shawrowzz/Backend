namespace DataServiceLayer.Models
{
    public class SimilarMovie
    {
        public string TConst { get; set; }
        public string PrimaryTitle { get; set; }
        public decimal SimilarityScore { get; set; }
    }
}