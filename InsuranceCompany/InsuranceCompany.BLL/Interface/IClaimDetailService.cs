using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;
using SharedModules;

public interface IClaimDetailService
{
    Task<IEnumerable<ClaimListOpenDTO>> ListAllOpenClaims();
    Task<IEnumerable<ClaimListOpenDTO>> ListAllClosedClaims();
    Task<ClaimListOpenDTO?> GetClaimByClaimId(string claimId);
    Task<CommonOutput> AddNewClaim(ClaimDetailRequestDTO claimDetail);
    Task<CommonOutput> UpdateClaim(string claimID, UpdateClaimDTO value);
    Task<CommonOutput> UpdateClaimSurveyorFees(string claimID);
    Task<CommonOutput> UpdateClaimAmtApprovedBySurveyor(string claimID, int claimant);
    Task<IEnumerable<ClaimStatusReportDTO>> ClaimStatusReportsBasedOnMonthAndYear(int month, int year);
    Task<ClaimPaymentReportDTO> PaymentStatusBasedOnMonthAndYear(int month, int year);
    Task<CommonOutput> UpdateAcceptRejectClaim(string claimId,bool acceptReject);

}
