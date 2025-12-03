// Summary:
// FeeRepository exposes a method to find a Fee entry that matches a given estimated loss.
// The repository uses AsNoTracking for read-only access and a range comparison to find the matching fee.
// FirstOrDefaultAsync returns either the matched Fee or null if none found.

using Microsoft.EntityFrameworkCore;

namespace InsuranceCompany.DAL;

public class FeeRepository : IFee
{

    public readonly InsuranceCompanyDBContext _dbcontext;
    //private readonly ILog _logger;

    public FeeRepository(InsuranceCompanyDBContext dbcontext)
    {
        _dbcontext = dbcontext;
        //_logger = logger;
    }

    // Returns the Fee whose [EstimateStartLimit, EstimateEndLimit) range contains the estimatedLoss.
    // Notes:
    // - The comparison EstimateStartLimit <= estimatedLoss && estimatedLoss < EstimateEndLimit defines a half-open interval.
    // - AsNoTracking() is used because the result is read-only, improving performance.
    // - FirstOrDefaultAsync() returns the first matching Fee or null if no match exists.
    public async Task<Fee?> GetFeesByEstimatedLoss(int estimatedLoss)
    {

        Fee? fees;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            fees= await _dbcontext.Fees.AsNoTracking().Where(cd=>cd.EstimateStartLimit<=estimatedLoss && estimatedLoss<cd.EstimateEndLimit).FirstOrDefaultAsync();

            //fees = await (from x in _dbcontext.Fees.AsNoTracking() where estimatedLoss >= x.EstimateStartLimit && estimatedLoss < x.EstimateEndLimit select x).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error("Ran with this problem " + ex.Message + " in FeeRepository");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used

        return fees;
    }

}
