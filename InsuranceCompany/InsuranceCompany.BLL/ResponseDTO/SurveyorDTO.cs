namespace InsuranceCompany.BLL;

public class SurveyorDTO
{
    public string SurveyorUserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int EstimateLimit { get; set; }
    public int TimesAllocated { get; set; }
}
