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
        public DateTime? LastExported { get; private set; }

        private static readonly string AutoSavePath = Path.Combine(
            Path.GetTempPath(), "interview_coach_autosave.json");

        public SessionService(InterviewService interviewService)
        {
            _interviewService = interviewService;
            _interviewService.RegisterAutoSave(AutoSave);
        }

        private JsonSerializerOptions GetOptions() => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public string ExportSession()
        {
            CurrentSession.ExportDate = DateTime.UtcNow;
            CurrentSession.Responses = _interviewService.GetAllResponses();
            LastExported = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(CurrentSession, GetOptions());
            ClearAutoSave();
            return json;
        }

        public void ImportSession(string json)
        {
            var session = JsonSerializer.Deserialize<SessionData>(json, GetOptions());
            if (session == null) return;
            CurrentSession = session;
            foreach (var response in session.Responses)
                _interviewService.SaveResponse(response);
        }

        public void AutoSave()
        {
            CurrentSession.Responses = _interviewService.GetAllResponses();
            var json = JsonSerializer.Serialize(CurrentSession, GetOptions());
            File.WriteAllText(AutoSavePath, json);
        }

        public bool HasAutoSave() => File.Exists(AutoSavePath);

        public void RestoreAutoSave()
        {
            if (!HasAutoSave()) return;
            var json = File.ReadAllText(AutoSavePath);
            ImportSession(json);
        }

        public void ClearAutoSave()
        {
            if (File.Exists(AutoSavePath))
                File.Delete(AutoSavePath);
        }
    }
}