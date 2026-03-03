namespace MiniProject_Take1.Domain.Enums
{

    public enum RoleType
    {
        SoftwareEngineer,
        AIEngineer,
        CloudSolutionArchitect,
        ProductManager,
        DevOpsEngineer,
        DevSecOpsEngineer,
        Other
    }

    public enum CompanyType
    {
        Google,
        Microsoft,
        Amazon,
        Meta,
        Apple,
        GovernmentContractor,
        ConsultingFirm,
        FinancialInstitution,
        Other
    }


    public enum QuestionType
    {
        Behavioral,
        Situational,
        Narrative,
        Technical, Coding,
    }

    public enum SourceType
    {
        LinkedIn,
        CompanySite,
        GoogleSearch,
        Indeed,
        Other
    }

    public enum ItemStatus
    {
        Todo,
        Wip,
        Solid,
        Mastered
    }

    public enum DifficultyLevel { Basic, Intermediate, Advanced }
}