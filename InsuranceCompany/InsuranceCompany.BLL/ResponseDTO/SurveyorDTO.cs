namespace InsuranceCompany.BLL;

public class SurveyorDTO
{
    public int SurveyorId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int EstimateLimit { get; set; }
    public int TimesAllocated { get; set; }
}
