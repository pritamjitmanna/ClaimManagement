using Microsoft.EntityFrameworkCore;

namespace InsuranceCompany.DAL;

public class SurveyorRepository : ISurveyor
{

    private readonly InsuranceCompanyDBContext _dbContext;
    //private readonly ILog _logger;

    public SurveyorRepository(InsuranceCompanyDBContext dbContext)
    {
        _dbContext = dbContext;
        //_logger = logger;
    }

    public async Task<Surveyor?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int estimatedLoss)
    {
        Surveyor? surveyor = null;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            surveyor = await (from x in _dbContext.Surveyors.AsNoTracking() where x.EstimateLimit == estimatedLoss orderby x.TimesAllocated select x).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorRepository");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return surveyor;
    }

    public async Task<IEnumerable<Surveyor>> GetAllSurveyorsForEstimatedLoss(int estimatedLoss)
    {

        IEnumerable<Surveyor> surveyors = new List<Surveyor>();
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            surveyors = await (from x in _dbContext.Surveyors.Include(s=>s.ClaimDetails).AsNoTracking() where x.EstimateLimit == estimatedLoss select x).ToListAsync();
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorRepository");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used

        return surveyors;
    }

    public async Task<Surveyor?> GetSurveyorById(int surveyorId)
    {


        Surveyor? surveyor;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            surveyor=await _dbContext.Surveyors.AsNoTracking().Where(cd=>cd.SurveyorId==surveyorId).FirstOrDefaultAsync();

            //surveyor = await (from x in _dbContext.Surveyors.AsNoTracking() where x.SurveyorId == surveyorId select x).FirstOrDefaultAsync();
            //if (surveyor == null) Console.WriteLine("True");
        }
        catch (Exception ex)
        {
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorRepository");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return surveyor;
    }
}
