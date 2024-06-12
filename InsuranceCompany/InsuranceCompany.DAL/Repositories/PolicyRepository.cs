using Microsoft.EntityFrameworkCore;

namespace InsuranceCompany.DAL;

public class PolicyRepository : IPolicy
{

    private readonly InsuranceCompanyDBContext _dbContext;
    //private readonly ILog _logger;


    public PolicyRepository(InsuranceCompanyDBContext dbContext)
    {
        _dbContext = dbContext;
        //_logger = logger;
    }

    public async Task<Policy?> GetPolicyByPolicyNo(string policyNo)
    {
        Policy? policy;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            policy = await _dbContext.Policies.AsNoTracking().Include(p=>p.ClaimDetails).Where(cd => cd.PolicyNo == policyNo).FirstOrDefaultAsync();
            //policy =await (from x in _dbContext.Policies.AsNoTracking() where x.PolicyNo == policyNo select x).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error("Ran with this problem " + ex.Message + " in PolicyRepository");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return policy;
    }
}
