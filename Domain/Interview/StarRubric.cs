namespace MiniProject_Take1.Domain.Interview
{
    public class StarRubric
    {
        public string? Section { get; set; }
        public string? Guidance { get; set; }
        public string GuidanceDetail { get; set; }
    }

    public class RubricLevel
    {
        public int Score { get; set; }
        public string? Description { get; set; }
    }
}