// Summary:
// This repository provides data-access methods related to Surveyor entities using Entity Framework Core.
// The methods use asynchronous EF Core LINQ queries and common query operators such as AsNoTracking (for read-only queries),
// Include (for eager-loading related navigation properties), orderby, FirstOrDefaultAsync and ToListAsync.
// Try/catch blocks are used to surface exceptions; commented logger calls indicate where logging would occur.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

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

    // Get a single surveyor whose EstimateLimit exactly matches estimatedLoss and, among such surveyors,
    // return the one with the minimum TimesAllocated (smallest times assigned) using ordering.
    // Notes on EF Core methods:
    // - AsNoTracking(): improves performance for read-only queries by disabling change tracking.
    // - from/where/orderby with LINQ is translated to SQL ORDER BY and WHERE clauses.
    // - FirstOrDefaultAsync(): asynchronously returns the first element or default (null) if none.
    public async Task<Surveyor?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int estimatedLoss)
    {
        Surveyor? surveyor = null;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            // The LINQ query will be translated by EF Core into SQL. AsNoTracking avoids tracking to reduce memory overhead.
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

    // Get all surveyors for an estimated loss value and eager-load related ClaimDetails.
    // Notes:
    // - Include(s => s.ClaimDetails): eager-loads the related ClaimDetails navigation property.
    // - ToListAsync(): executes the query and asynchronously materializes a List<T>.
    public async Task<IEnumerable<Surveyor>> GetAllSurveyorsForEstimatedLoss(int estimatedLoss)
    {

        IEnumerable<Surveyor> surveyors = new List<Surveyor>();
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            // AsNoTracking combined with Include is fine for read-only scenarios and avoids creating tracked entities.
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

    // Get a single Surveyor by its primary key SurveyorId.
    // Notes:
    // - Where + FirstOrDefaultAsync() filters and returns a single item or null if not found.
    // - Using AsNoTracking avoids change tracking when only reading.
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

    public async Task<CommonOutput> AddSurveyorDetails(Surveyor surveyor)
    {
        CommonOutput result;
        try{
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            bool IsValid=ValidationFunctions.ValidateModel(surveyor,ref validationResults);
            if (!IsValid)
            {
                result=new CommonOutput
                {
                    Result=RESULT.FAILURE,
                    Output=validationResults
                };
            }
            else{
                await _dbContext.Surveyors.AddAsync(surveyor);
                await _dbContext.SaveChangesAsync();
                result=new CommonOutput
                {
                    Result=RESULT.SUCCESS,
                    Output=surveyor.SurveyorId
                };
            }
            
        }
        catch(Exception ex)
        {
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorRepository");
            throw;
        }
        return result;
    }

    public async Task<bool> DeleteSurveyorDetails(int surveyorId)
    {
        bool isDeleted=false;
        try{
            var surveyor=await GetSurveyorById(surveyorId);
            if(surveyor!=null){
                _dbContext.Surveyors.Remove(surveyor);
                await _dbContext.SaveChangesAsync();
                isDeleted=true;
            }
        }
        catch(Exception ex)
        {
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorRepository");
            throw;
        }
        return isDeleted;
    } 
}
