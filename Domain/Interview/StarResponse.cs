using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    // Behavioral + Situational
    public class StarResponse : InterviewResponse
    {
        public string Situation { get; set; }
        public string? Task { get; set; }
        public string? Action { get; set; }
        public string? Result { get; set; }
        public float SituationScore { get; set; }
        public float TaskScore { get; set; }
        public float ActionScore { get; set; }
        public float ResultScore { get; set; }
        public float OverallScore => (SituationScore + TaskScore + ActionScore + ResultScore) / 4;
    }
}
