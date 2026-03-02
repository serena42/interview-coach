using MiniProject_Take1.Domain.Interview;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniProject_Take1.Services
{
    public class InterviewService
    {
        private List<InterviewQuestion> _questions = new();
        private List<InterviewResponse> _responses = new();


        public async Task LoadQuestionsAsync(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"Questions file not found at: {jsonPath}");

            var json = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            _questions = JsonSerializer.Deserialize<List<InterviewQuestion>>(json, options)
             ?? new List<InterviewQuestion>();

            Console.WriteLine($"Loaded {_questions.Count} questions");
        }

        public List<InterviewQuestion> GetAllQuestions() => _questions;

        public List<InterviewQuestion> GetByType(QuestionType type) =>
            _questions.Where(q => q.QuestionType == type).ToList();
        public void UpdateResponse(InterviewResponse response)
        {
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
        }
        public void UpdateStatus(InterviewResponse response)
        {
            if (response is StarResponse star)
            {
                if (response.Status == ItemStatus.Todo)
                    response.Status = ItemStatus.Wip;
                else if (response.Status == ItemStatus.Wip && star.OverallScore >= 7)
                    response.Status = ItemStatus.Solid;
                else if (response.Status == ItemStatus.Solid && star.OverallScore >= 9)
                    response.Status = ItemStatus.Mastered;
            }
            else if (response is FreeformResponse free)
            {
                if (response.Status == ItemStatus.Todo)
                    response.Status = ItemStatus.Wip;
                else if (response.Status == ItemStatus.Wip && free.Score >= 7)
                    response.Status = ItemStatus.Solid;
                else if (response.Status == ItemStatus.Solid && free.Score >= 9)
                    response.Status = ItemStatus.Mastered;
            }
            else
            {
                // future types (Technical, Coding, etc.)
                if (response.Status == ItemStatus.Todo)
                    response.Status = ItemStatus.Wip;
            }
        }
        public void SaveResponse(InterviewResponse response)
        {
            response.Id = Guid.NewGuid().ToString();
            response.LastEdited = DateTime.UtcNow;
            UpdateResponse(response);
            _responses.Add(response);
        }

        public List<InterviewResponse> GetAllResponses() => _responses;
    }
}