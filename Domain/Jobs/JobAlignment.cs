namespace MiniProject_Take1.Domain.Jobs
{
    public class JobAlignmentRubric
    {
        public string JobId { get; set; }
        public List<JobAlignmentCategory> Categories { get; set; }
    }

    public class JobAlignmentCategory
    {
        public string Name { get; set; } // e.g., "Technical Skills", "Experience Level", "Communication"
        public string Description { get; set; } // What this category evaluates
        public List<JobAlignmentCriterion> Criteria { get; set; }
    }

    public class JobAlignmentCriterion
    {
        public string Prompt { get; set; } // e.g., "Does the résumé demonstrate required programming languages?"
        public string MatchType { get; set; } // e.g., "Exact", "Fuzzy", "Inferred"
        public string Source { get; set; } // e.g., "Résumé", "LinkedInProfile"
        public int Score { get; set; } // 1–5
        public string Justification { get; set; } // Optional notes or evidence
    }



}
