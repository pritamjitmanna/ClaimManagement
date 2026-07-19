using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;
using SharedModules;

public interface IClaimDetailService
{
    Task<IEnumerable<ClaimListOpenDTO>> ListAllOpenClaims(string userId,List<string> roles);
    Task<IEnumerable<ClaimListOpenDTO>> ListAllClosedClaims(string userId,List<string> roles);
    Task<CommonOutput> GetClaimByClaimId(string userId,List<string> roles, string claimId);
    Task<CommonOutput> AddNewClaim(string userId,List<string> roles,ClaimDetailRequestDTO claimDetail);
    Task<CommonOutput> UpdateClaim(string claimID, UpdateClaimDTO value);
    Task<CommonOutput> UpdateClaimSurveyorFees(string claimID);
    Task<CommonOutput> UpdateClaimAmtApprovedBySurveyor(string claimID, int claimant);
    Task<IEnumerable<ClaimStatusReportDTO>> ClaimStatusReportsBasedOnMonthAndYear(int month, int year);
    Task<ClaimPaymentReportDTO> PaymentStatusBasedOnMonthAndYear(int month, int year);
    Task<CommonOutput> UpdateAcceptRejectClaim(string claimId,bool acceptReject);

}
