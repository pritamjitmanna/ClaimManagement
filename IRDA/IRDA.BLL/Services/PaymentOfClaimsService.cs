using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

public class PaymentOfClaimsService:IPaymentOfClaimsService
{
    private readonly ClaimsService.ClaimsServiceClient _claimsService; 
    private readonly IPaymentOfClaims _paymentRepository;
    private readonly IMapper _mapper;
    public PaymentOfClaimsService(ClaimsService.ClaimsServiceClient claimsService,IPaymentOfClaims paymentRepository,IMapper mapper){
        _claimsService = claimsService;
        _mapper = mapper;
        _paymentRepository=paymentRepository;
    }

    public async Task<CommonOutput> AddPaymentOfClaimsStatus(int month,int year){

        CommonOutput result;
        try{

            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }


            CommonOutputgRPC output=await _claimsService.GetPaymentStatusReportsAsync(new GetPaymentStatusReportsMonthAndYear{Month=month,Year=year});


            if(output.StatusCode==STATUSCODE.Ok){

                if(output.Output.TryUnpack(out ClaimPaymentReportDTOgRPC report)){
                    PaymentOfClaims payment=_mapper.Map<PaymentOfClaims>(report);
                    


                    PaymentOfClaims? alreadyPresent=await _paymentRepository.PaymentStatusOnMonthAndYear(month,year);

                    if(alreadyPresent!=null){
                        alreadyPresent.Payment=payment.Payment;
                        result=await _paymentRepository.UpdatePaymentOfClaimsData(alreadyPresent);
                    }
                    else{
                        payment.ReportId=GenerateReportId(payment);
                        result=await _paymentRepository.AddPaymentOfClaimsData(payment);
                    }

                    GetErrorListInRequiredFormat(ref result);
                }
                else{
                    throw new Exception();
                }

            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                if(output.Output.TryUnpack(out StringValue val)){
                    result=new CommonOutput{
                        Result=RESULT.FAILURE,
                        Output= val.Value
                    };
                }
                else{
                    throw new Exception();
                }
            }
            else if(output.StatusCode==STATUSCODE.Internalservererror){
                throw new Exception();
            }
            else{
                result=new CommonOutput{Result=RESULT.FAILURE};
            }

        }
        catch(InvalidMonthOrYearException ex){
            return new CommonOutput{
                Result=RESULT.FAILURE,
                Output=ex.Message
            };
        }
        catch(Exception ex){
            throw;
        }
        return result;

    }

    public async Task<ClaimPaymentReportDTO> GetPaymentStatus(int month,int year){
        PaymentOfClaims? totalPayment;
        try
        {
            totalPayment = await _paymentRepository.PaymentStatusOnMonthAndYear(month, year);
        }
        catch (Exception ex)
        {
            //Log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return new ClaimPaymentReportDTO
        {
            Month = month,
            Year = year,
            Amount = totalPayment==null?0:totalPayment.Payment,
        };
    }


    private static string GenerateReportId(PaymentOfClaims payment){
        return string.Concat("PS", payment.Month[..3].ToUpper(), payment.Year.ToString().Substring(2,2));
    }

    private void GetErrorListInRequiredFormat(ref CommonOutput result)
    {
        if (result.Result == RESULT.FAILURE)
        {
            List<PropertyValidationResponse> validationErrors = new List<PropertyValidationResponse>();

            foreach (var err in (ICollection<ValidationResult>)result.Output)
            {
                validationErrors.Add(
                    new PropertyValidationResponse
                    {
                        Property = err.MemberNames.First(),
                        ErrorMessage = err.ErrorMessage
                    });
            }

            result.Output = validationErrors;
        }
    }
}
