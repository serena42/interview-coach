using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    public class FreeFormResponse
    {
        // Narrative + Technical
        public class FreeformResponse : InterviewResponse
        {
            public string Content { get; set; }
        }
    }
}
