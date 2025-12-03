// Summary:
// PolicyService is a thin service that delegates policy retrieval to IPolicy repository.
// It uses async/await and forwards exceptions. The service shields higher layers from direct DAL access.

using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;

public class PolicyService : IPolicyService
{
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
    private readonly IPolicy _policyRepository;
    //private readonly ILog _logger;

    public PolicyService(IPolicy policyRepository)
    {
        _policyRepository = policyRepository;
        //_logger = logger;
    }

    // GetPolicyByPolicyNo:
    // - Delegates to repository GetPolicyByPolicyNo (which uses EF Include and AsNoTracking).
    // - Uses await to asynchronously obtain result and returns Policy or null.
    public async Task<Policy?> GetPolicyByPolicyNo(string policyNo)
    {

        //if(policyNo == null)

        Policy? policy;


        try
        {
            policy = await _policyRepository.GetPolicyByPolicyNo(policyNo);
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in PolicyService");
            throw;
        }

        return policy;
    }


}

