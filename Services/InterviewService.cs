using MiniProject_Take1.Domain.Interview;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniProject_Take1.Domain.Enums;

namespace MiniProject_Take1.Services
{
    public class InterviewService
    {
        private List<InterviewQuestion> _questions = new();
        private List<InterviewResponse> _responses = new();

        public async Task LoadQuestionsAsync(params string[] jsonPaths)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            _questions = new List<InterviewQuestion>();

            foreach (var jsonPath in jsonPaths)
            {
                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"Questions file not found at: {jsonPath}");

                var json = await File.ReadAllTextAsync(jsonPath);
                var loaded = JsonSerializer.Deserialize<List<InterviewQuestion>>(json, options)
                    ?? new List<InterviewQuestion>();

                _questions.AddRange(loaded);
                Console.WriteLine($"Loaded {loaded.Count} questions from {Path.GetFileName(jsonPath)}");
            }

            Console.WriteLine($"Total questions loaded: {_questions.Count}");
        }

        public List<InterviewQuestion> GetAllQuestions() => _questions;

        public List<InterviewQuestion> GetByType(QuestionType type) =>
            _questions.Where(q => q.QuestionType == type).ToList();


        public List<InterviewQuestion> GetNewForReview()
        {
            var answeredIds = _responses
                .Select(r => r.QuestionId)
                .ToHashSet();

            return _questions
                .Where(q => q.QuestionType == QuestionType.Technical || q.QuestionType == QuestionType.Coding)
                .Where(q => !answeredIds.Contains(q.Id))
                .ToList();
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
        public List<InterviewResponse> GetDueForReview()
        {
            return _responses
                .Where(r => r.QuestionType == QuestionType.Technical || r.QuestionType == QuestionType.Coding)
                .Where(r => r.NextReviewDate == null || r.NextReviewDate <= DateTime.UtcNow)
                .OrderBy(r => r.NextReviewDate ?? DateTime.MinValue)
                .ToList();
        }
        public void UpdateReviewSchedule(InterviewResponse response, int rating)
        {
            response.ReviewInterval = rating switch
            {
                1 or 2 => 1,
                3 => 3,
                4 => Math.Max(1, response.ReviewInterval * 2),
                5 => Math.Max(1, response.ReviewInterval * 3),
                _ => 1
            };
            response.NextReviewDate = DateTime.UtcNow.AddDays(response.ReviewInterval);

            if (response is FreeformResponse free)
            {
                // Map 1-5 rating to 1-10 score so status promotion logic works
                free.Score = rating * 2;
            }
        }
        public void SaveResponse(InterviewResponse response)
        {
            if (string.IsNullOrEmpty(response.Id))
                response.Id = Guid.NewGuid().ToString();
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
            _responses.Add(response);
        }
        public void UpdateResponse(InterviewResponse response)
        {
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
        }

        public void UpdateResponse(InterviewResponse response)
        {
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
        }

        public List<InterviewResponse> GetAllResponses() => _responses;

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
        }
    }
}