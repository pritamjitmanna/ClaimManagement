// Summary:
// PolicyRepository provides read access to Policy entities. The GetPolicyByPolicyNo method
// uses Include to eager-load the ClaimDetails navigation property and AsNoTracking for read-only access.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

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

    // Fetch a policy by PolicyNo while eager-loading related ClaimDetails.
    // - Include(p => p.ClaimDetails): loads child entities in the same query to avoid lazy-loading round-trips.
    // - AsNoTracking(): better performance for read-only use.
    // - FirstOrDefaultAsync(): returns the matched Policy or null.
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

    public async Task<CommonOutput> AddNewPolicy(Policy policy)
    {
        CommonOutput result;
        try
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            bool IsValid = ValidationFunctions.ValidateModel(policy, ref validationResults);
            if (!IsValid)
            {
                result=new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output=validationResults
                };
            }
            else
            {
                await _dbContext.Policies.AddAsync(policy);
                await _dbContext.SaveChangesAsync();
                result=new CommonOutput
                {
                    Result = RESULT.SUCCESS,
                    Output = policy.PolicyNo
                };
            }
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error("Ran with this problem " + ex.Message + " in PolicyRepository");
            throw;
        }
        return result;
    }
}