using SharedModules;

namespace Insured.BLL;

public interface IInsuredService
{
    Task<CommonOutput> AddNewClaim(ClaimDetailRequestDTO claim);

    Task<CommonOutput> AcceptOrRejectClaim(string claimId,AcceptRejectDTO acceptReject);
}
