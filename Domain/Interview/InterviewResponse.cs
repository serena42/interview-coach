using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    // Base - every question type gets this
    public class InterviewResponse
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }      // links back to the question
        public QuestionType QuestionType { get; set; }
        public DateTime LastEdited { get; set; }
        public ConfidenceLevel Confidence { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
    }


}
