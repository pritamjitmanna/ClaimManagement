using SharedModules;

namespace Insured.BLL;

public interface IInsuredService
{
    Task<CommonOutput> AddNewClaim(string token,ClaimDetailRequestDTO claim);

    Task<CommonOutput> AcceptOrRejectClaim(string claimId,AcceptRejectDTO acceptReject);

    Task<CommonOutput> AddNewPolicy(string token,PolicyEntryDTO policy);
    Task<CommonOutput> GetPolicyByPolicyNumber(string token,string policyNumber);

}
