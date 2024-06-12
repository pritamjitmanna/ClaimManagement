using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class ClaimListOpenDTO
{
    public required string ClaimId { get; set; }
    public required string PolicyNo { get; set; }
    public int EstimatedLoss { get; set; }
    public DateOnly DateOfAccident { get; set; }
    public int SurveyorID { get; set; }
    public int AmtApprovedBySurveyor { get; set; }
    public bool InsuranceCompanyApproval { get; set; }
    public WITHDRAWSTATUS WithdrawClaim { get; set; }
    public ClaimStatus ClaimStatus { get; set; }
    public int? SurveyorFees {  get; set; }

}
