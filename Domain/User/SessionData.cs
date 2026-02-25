using MiniProject_Take1.Domain.Interview;
using MiniProject_Take1.Domain.LinkedIn;
using MiniProject_Take1.Domain.Jobs;

namespace MiniProject_Take1.Domain.User
{
    public class SessionData
    {
        public DateTime ExportDate { get; set; }
        public List<InterviewResponse> Responses { get; set; } = new();
        public LinkedInProfile LinkedInProfile { get; set; }
        public string Resume { get; set; }
        public List<JobAlignmentRubric> JobDescriptions { get; set; } = new();
    }
}