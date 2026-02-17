using System;

namespace InsuranceCompany.BLL.RequestDTO;

public class SurveyorEntryDTO
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int EstimateLimit { get; set; }
}
