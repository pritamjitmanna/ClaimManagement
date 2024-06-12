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
