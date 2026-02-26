using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    // Behavioral + Situational
    public class StarResponse : InterviewResponse
    {
        public string Situation { get; set; }
        public string Task { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public int SituationScore { get; set; }  // 1-5
        public int TaskScore { get; set; }
        public int ActionScore { get; set; }
        public int ResultScore { get; set; }
        public int OverallScore => (SituationScore + TaskScore + ActionScore + ResultScore) / 4;
    }
}
