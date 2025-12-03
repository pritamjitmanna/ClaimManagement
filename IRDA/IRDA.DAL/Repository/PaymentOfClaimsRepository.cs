using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace IRDA.DAL;

/// <summary>
/// Repository implementation for PaymentOfClaims persistence.
/// - Performs model validation using ValidationFunctions before insert/update.
/// - Uses EF Core DbContext to add, update and query PaymentOfClaims entities.
/// </summary>
public class PaymentOfClaimsRepository:IPaymentOfClaims
{
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly IRDADBContext _dbcontext;
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public PaymentOfClaimsRepository(IRDADBContext dbcontext){
        _dbcontext = dbcontext;
    }

    /// <summary>
    /// Adds a PaymentOfClaims after validating the entity.
    /// - Uses ValidationFunctions.ValidateModel which leverages DataAnnotations and Validator.TryValidateObject.
    /// - On success adds entity via DbContext.AddAsync and SaveChangesAsync.
    /// - Returns CommonOutput with FAILURE and the ValidationResult collection when validation fails.
    /// </summary>
    public async Task<CommonOutput> AddPaymentOfClaimsData(PaymentOfClaims payment){
        CommonOutput result;
        try{
            ICollection<ValidationResult> results=[];
            bool IsValid=ValidationFunctions.ValidateModel(payment,ref results);
            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{
                
                await _dbcontext.AddAsync(payment);
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
    /// Updates a PaymentOfClaims after validation and persists changes.
    /// </summary>
    public async Task<CommonOutput> UpdatePaymentOfClaimsData(PaymentOfClaims payment){
        CommonOutput result;
        try{
            ICollection<ValidationResult> results=[];
            bool IsValid=ValidationFunctions.ValidateModel(payment,ref results);
            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{
                
                _dbcontext.Update(payment);
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
    /// Queries the database for a PaymentOfClaims entry matching the provided month and year.
    /// - Uses AsNoTracking() for read-only query to reduce EF tracking overhead.
    /// - Month parameter is translated to the month name using the MonthName array.
    /// </summary>
    public async Task<PaymentOfClaims?> PaymentStatusOnMonthAndYear(int month, int year)
    {

        PaymentOfClaims? totalPayment;

        try
        {
            totalPayment = await _dbcontext.PaymentClaims.AsNoTracking().Where(cd=>cd.Month==MonthName[month-1]).Where(cd=>cd.Year==year).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log Error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }

        return totalPayment;
    }


}
