using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany;

public interface ISharedLogic
{
    Task<CommonOutput> AddClaimSharedLogic(string userId,List<string> roles,ClaimDetailRequestDTO claimDetail);
    Task<CommonOutput> GetClaimByClaimId(string userId,List<string> roles, string claimId);
    Task<CommonOutput> GetClaimStatusReports(int month, int year);

    Task<CommonOutput> GetPaymentStatusReports(int month, int year);

    Task<CommonOutput> UpdateClaimAmountApprovedBySurveyor(string claimID, int claimant);
}
