using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

public class Surveyor
{
    public int SurveyorId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "The Estimated Limit must be a positive number")]
    public int EstimateLimit { get; set; }
    public int TimesAllocated { get; set; }
    public ICollection<ClaimDetail> ClaimDetails { get; set; }

}
