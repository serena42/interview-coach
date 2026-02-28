using MiniProject_Take1.Domain.Enums;
using System.Text.Json.Serialization;

namespace MiniProject_Take1.Domain.Interview
{
    //serialization is converting an object into memory, deserialization is reading it back into an object
    [JsonDerivedType(typeof(StarResponse), typeDiscriminator: "star")] //fixes polymorphic serialization
    [JsonDerivedType(typeof(FreeformResponse), typeDiscriminator: "freeform")] // which is when a serializer doesn't know about the derived classes
    public class InterviewResponse
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public DateTime LastEdited { get; set; }
        public ConfidenceLevel Confidence { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public ItemStatus Status { get; set; }
        public DateTime? Deadline { get; set; }
    }
}