
using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    public class InterviewQuestion
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string Competency { get; set; }
        public QuestionType QuestionType { get; set; }
    }
}
