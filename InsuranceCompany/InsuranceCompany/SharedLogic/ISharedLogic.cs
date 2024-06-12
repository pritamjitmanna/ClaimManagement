using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany;

public interface ISharedLogic
{
    Task<CommonOutput> AddClaimSharedLogic(ClaimDetailRequestDTO claimDetail);
    Task<CommonOutput> GetClaimByClaimId(string ClaimId);
    Task<CommonOutput> GetClaimStatusReports(int month, int year);

    Task<CommonOutput> GetPaymentStatusReports(int month, int year);

    Task<CommonOutput> UpdateClaimAmountApprovedBySurveyor(string claimID, int claimant);
}
