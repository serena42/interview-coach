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
        private Action? _onSave;

        public void RegisterAutoSave(Action onSave)
        {
            _onSave = onSave;
        }

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


        public List<InterviewQuestion> GetNewForReview(QuestionType questionType)
        {
            var answeredIds = _responses.Select(r => r.QuestionId).ToHashSet();
            return _questions
                .Where(q => q.QuestionType == questionType && !answeredIds.Contains(q.Id))
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
        public List<InterviewResponse> GetDueForReview(QuestionType questionType)
        {
            return _responses
                .Where(r => r.QuestionType == questionType)
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
            response.Id = Guid.NewGuid().ToString();
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
            _responses.Add(response);
            _onSave?.Invoke();
        }

        public void UpdateResponse(InterviewResponse response)
        {
            response.LastEdited = DateTime.UtcNow;
            UpdateStatus(response);
            _onSave?.Invoke();
        }

        public List<InterviewResponse> GetAllResponses() => _responses;
        public List<(InterviewQuestion Question, InterviewResponse? Response)> BuildReviewSession(int newCardCount, QuestionType questionType)
        {
            var questionsById = _questions.ToDictionary(q => q.Id);
            var answeredIds = _responses.Select(r => r.QuestionId).ToHashSet();

            var dueResponses = GetDueForReview(questionType);
            var dueCards = dueResponses
                .Select(r =>
                {
                    questionsById.TryGetValue(r.QuestionId, out var question);
                    return (Question: question, Response: (InterviewResponse?)r);
                })
                .Where(x => x.Question != null)
                .ToList();

            var basicDone = !_questions
                .Any(q => q.QuestionType == questionType
                       && q.Difficulty == DifficultyLevel.Basic
                       && !answeredIds.Contains(q.Id));

            var intermediateDone = !_questions
                .Any(q => q.QuestionType == questionType
                       && q.Difficulty == DifficultyLevel.Intermediate
                       && !answeredIds.Contains(q.Id));

            var unlockedDifficulty = basicDone
                ? (intermediateDone ? DifficultyLevel.Advanced : DifficultyLevel.Intermediate)
                : DifficultyLevel.Basic;

            var maxNew = Math.Max(0, 20 - dueCards.Count);
            var actualNewCount = dueCards.Count >= 20 ? 0 : Math.Min(newCardCount, maxNew);

            if (dueCards.Count + actualNewCount < 5)
                actualNewCount = Math.Min(5 - dueCards.Count,
                    _questions.Count(q =>
                        q.QuestionType == questionType
                        && q.Difficulty == unlockedDifficulty
                        && !answeredIds.Contains(q.Id)));

            var newCards = _questions
                .Where(q => q.QuestionType == questionType
                         && q.Difficulty == unlockedDifficulty
                         && !answeredIds.Contains(q.Id))
                .Take(actualNewCount)
                .Select(q => (Question: q, Response: (InterviewResponse?)null))
                .ToList();

            var shuffledDue = dueCards
                .GroupBy(x => x.Question!.Difficulty)
                .OrderBy(g => g.Key switch
                {
                    DifficultyLevel.Basic => 0,
                    DifficultyLevel.Intermediate => 1,
                    DifficultyLevel.Advanced => 2,
                    _ => 3
                })
                .SelectMany(g => g.OrderBy(_ => Random.Shared.Next()))
                .ToList();

            return shuffledDue.Concat(newCards).ToList();
        }
    }
}