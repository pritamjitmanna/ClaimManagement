using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace IRDA.DAL;

public class PendingStatusReportsRepository:IPendingStatusReports
{
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly IRDADBContext _dbcontext;
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public PendingStatusReportsRepository(IRDADBContext dbcontext){
        _dbcontext = dbcontext;
    }

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
