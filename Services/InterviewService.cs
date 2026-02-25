using System.Text.Json;
using MiniProject_Take1.Domain.Interview;

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
                PropertyNameCaseInsensitive = true
            };

            _questions = JsonSerializer.Deserialize<List<InterviewQuestion>>(json, options)
                         ?? new List<InterviewQuestion>();

            Console.WriteLine($"Loaded {_questions.Count} questions");
        }

        public List<InterviewQuestion> GetAllQuestions() => _questions;

        public List<InterviewQuestion> GetByType(QuestionType type) =>
            _questions.Where(q => q.QuestionType == type).ToList();

        public void SaveResponse(InterviewResponse response)
        {
            response.Id = Guid.NewGuid().ToString();
            response.LastEdited = DateTime.UtcNow;
            _responses.Add(response);
        }

        public List<InterviewResponse> GetAllResponses() => _responses;
    }
}