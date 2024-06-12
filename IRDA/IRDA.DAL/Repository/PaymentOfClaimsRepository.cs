using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace IRDA.DAL;

public class PaymentOfClaimsRepository:IPaymentOfClaims
{
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly IRDADBContext _dbcontext;
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public PaymentOfClaimsRepository(IRDADBContext dbcontext){
        _dbcontext = dbcontext;
    }

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
