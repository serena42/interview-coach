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
        public void UpdateResponse(InterviewResponse response)
        {
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
        }

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
            UpdateResponse(response);
        }
        public void SaveResponse(InterviewResponse response)
        {
            response.Id = Guid.NewGuid().ToString();
            response.LastEdited = DateTime.UtcNow;
            UpdateResponse(response);
            _responses.Add(response);
        }

        public List<InterviewResponse> GetAllResponses() => _responses;
        public List<(InterviewQuestion Question, InterviewResponse? Response)> BuildReviewSession(int newCardCount)
        {
            var answeredIds = _responses
                .Select(r => r.QuestionId)
                .ToHashSet();

            // Due cards — already have a response, past due date
            var dueResponses = GetDueForReview();
            var dueCards = dueResponses
                .Select(r => (
                    Question: _questions.FirstOrDefault(q => q.Id == r.QuestionId),
                    Response: (InterviewResponse?)r
                ))
                .Where(x => x.Question != null)
                .ToList();

            // Determine which difficulty is currently unlocked for new cards
            var basicDone = !_questions
                .Any(q => (q.QuestionType == QuestionType.Technical || q.QuestionType == QuestionType.Coding)
                       && q.Difficulty == DifficultyLevel.Basic
                       && !answeredIds.Contains(q.Id));

            var intermediateDone = !_questions
                .Any(q => (q.QuestionType == QuestionType.Technical || q.QuestionType == QuestionType.Coding)
                       && q.Difficulty == DifficultyLevel.Intermediate
                       && !answeredIds.Contains(q.Id));

            var unlockedDifficulty = basicDone
                ? (intermediateDone ? DifficultyLevel.Advanced : DifficultyLevel.Intermediate)
                : DifficultyLevel.Basic;

            // Cap new cards so total doesn't exceed 20, unless reviews alone exceed 20
            var maxNew = Math.Max(0, 20 - dueCards.Count);
            var actualNewCount = dueCards.Count >= 20
                ? 0
                : Math.Min(newCardCount, maxNew);

            // Minimum 5 total
            if (dueCards.Count + actualNewCount < 5)
                actualNewCount = Math.Min(5 - dueCards.Count,
                    _questions.Count(q =>
                        (q.QuestionType == QuestionType.Technical || q.QuestionType == QuestionType.Coding)
                        && q.Difficulty == unlockedDifficulty
                        && !answeredIds.Contains(q.Id)));

            // Get new cards at unlocked difficulty
            var newCards = _questions
                .Where(q => (q.QuestionType == QuestionType.Technical || q.QuestionType == QuestionType.Coding)
                         && q.Difficulty == unlockedDifficulty
                         && !answeredIds.Contains(q.Id))
                .Take(actualNewCount)
                .Select(q => (Question: q, Response: (InterviewResponse?)null))
                .ToList();

            // Shuffle due cards within difficulty groups, then append new cards
            var rng = new Random();
            var shuffledDue = dueCards
                .GroupBy(x => x.Question!.Difficulty)
                .OrderBy(g => g.Key switch
                {
                    DifficultyLevel.Basic => 0,
                    DifficultyLevel.Intermediate => 1,
                    DifficultyLevel.Advanced => 2,
                    _ => 3
                })
                .SelectMany(g => g.OrderBy(_ => rng.Next()))
                .ToList();

            return shuffledDue.Concat(newCards).ToList();
        }
    }
}