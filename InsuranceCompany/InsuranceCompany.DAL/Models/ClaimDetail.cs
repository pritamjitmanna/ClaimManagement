using System.ComponentModel.DataAnnotations;
using SharedModules;

namespace InsuranceCompany.DAL;

public class ClaimDetail
{
    [Length(10, 10, ErrorMessage = "The length of the ClaimId must be 10 characters.")]
    public required string ClaimId { get; set; }
    public string PolicyNo { get; set; }
    public Policy? Policy { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "EstimatedLoss must be non-negative")]
    public int EstimatedLoss { get; set; }

    [DateLessThanCurrent(ErrorMessage = "The DateOfAccident must be less than current date.")]
    public DateOnly DateOfAccident { get; set; }
    public ClaimStatus ClaimStatus { get; set; }

    public int? SurveyorID { get; set; }
    public Surveyor? Surveyor { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "AmountApprovedBySurveyor must be non-negative")]
    public int? AmtApprovedBySurveyor { get; set; }//
    public bool InsuranceCompanyApproval { get; set; }
    public WITHDRAWSTATUS WithdrawClaim { get; set; }
    public int? SurveyorFees { get; set; }//

}
