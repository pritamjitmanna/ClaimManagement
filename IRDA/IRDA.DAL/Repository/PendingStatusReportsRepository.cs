using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace IRDA.DAL;

/// <summary>
/// Repository implementation for PendingStatusReports persistence.
/// - Validates entities prior to insert/update using ValidationFunctions.
/// - Performs EF Core queries and uses AsNoTracking for read operations.
/// </summary>
public class PendingStatusReportsRepository:IPendingStatusReports
{
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly IRDADBContext _dbcontext;
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public PendingStatusReportsRepository(IRDADBContext dbcontext){
        _dbcontext = dbcontext;
    }

    /// <summary>
    /// Validates and inserts a new PendingStatusReports record.
    /// - On validation failure returns CommonOutput with ValidationResult collection.
    /// </summary>
    public async Task<CommonOutput> AddPendingStatusReportsData(PendingStatusReports status){
        CommonOutput result;
        try{
            ICollection<ValidationResult> results=[];
            bool IsValid=ValidationFunctions.ValidateModel(status,ref results);
            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{
                
                await _dbcontext.AddAsync(status);
                await _dbcontext.SaveChangesAsync();
                result = new CommonOutput
                {
                    Result = RESULT.SUCCESS
                };
            }
        }
        catch(Exception ex){
            throw;
        }

        return result;
    }

    /// <summary>
    /// Validates and updates an existing PendingStatusReports record.
    /// </summary>
    public async Task<CommonOutput> UpdatePendingStatusReportsData(PendingStatusReports status){
        CommonOutput result;
        try{
            ICollection<ValidationResult> results=[];
            bool IsValid=ValidationFunctions.ValidateModel(status,ref results);
            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{
                
                _dbcontext.Update(status);
                await _dbcontext.SaveChangesAsync();
                result = new CommonOutput
                {
                    Result = RESULT.SUCCESS
                };
            }
        }
        catch(Exception ex){
            throw;
        }

        return result;
    }


    /// <summary>
    /// Retrieves a PendingStatusReports entry by stage, month and year.
    /// - Uses AsNoTracking and translates month index to month name via MonthName array.
    /// </summary>
    public async Task<PendingStatusReports?> PendingStatusReportsOnMonthAndYear(Stages stage,int month, int year)
    {

        PendingStatusReports? statusReport;

        try
        {
            statusReport = await _dbcontext.StatusReports.AsNoTracking().Where(cd=>cd.Month==MonthName[month-1]).Where(cd=>cd.Year==year).Where(cd=>cd.Stage==stage).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log Error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }

        return statusReport;
    }
}
