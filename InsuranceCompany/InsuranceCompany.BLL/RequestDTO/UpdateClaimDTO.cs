using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class UpdateClaimDTO
{
    public ClaimStatus? ClaimStatus { get; set; }
    public int? SurveyorID { get; set; }
    public bool? InsuranceCompanyApproval { get; set; }
}
