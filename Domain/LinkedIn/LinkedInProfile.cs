namespace MiniProject_Take1.Domain.LinkedIn
{
    public class LinkedInProfile
    {
        public string FullName { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public List<LinkedInPosition> Positions { get; set; } = new();
        public List<LinkedInEducation> Education { get; set; } = new();
        public List<LinkedInSkill> Skills { get; set; } = new();
        public List<LinkedInCertification> Certifications { get; set; } = new();
    }
    public class LinkedInPosition
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }

    public class LinkedInEducation
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime? GraduationDate { get; set; }
    }

    public class LinkedInSkill
    {
        public string Name { get; set; }
        public int Endorsements { get; set; }
    }

    public class LinkedInCertification
    {
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public DateTime? IssueDate { get; set; }
    }


}