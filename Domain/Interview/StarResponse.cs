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
    }
}
