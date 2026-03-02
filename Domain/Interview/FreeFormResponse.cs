using MiniProject_Take1.Domain.Enums;
namespace MiniProject_Take1.Domain.Interview
{
    public class FreeformResponse : InterviewResponse
    {
        // Narrative 
                public string Content { get; set; }
                public float Score { get; set; }

    }
}
