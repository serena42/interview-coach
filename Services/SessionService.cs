using MiniProject_Take1.Domain.Interview;
using MiniProject_Take1.Domain.User;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniProject_Take1.Services
{
    public class SessionService
    {
        private readonly InterviewService _interviewService;

        public SessionData CurrentSession { get; private set; } = new();

        public SessionService(InterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public string ExportSession()
        {
            CurrentSession.ExportDate = DateTime.UtcNow;
            CurrentSession.Responses = _interviewService.GetAllResponses();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Serialize(CurrentSession, options);
        }

        public void ImportSession(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var session = JsonSerializer.Deserialize<SessionData>(json, options);
            if (session == null) return;

            CurrentSession = session;

            foreach (var response in session.Responses)
                _interviewService.SaveResponse(response);
        }
    }
}