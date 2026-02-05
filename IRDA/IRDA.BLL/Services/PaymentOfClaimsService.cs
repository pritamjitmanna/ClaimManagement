using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using gRPCSharedProtos.Protos;
using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

/// <summary>
/// Service responsible for pulling payment status reports from the remote Claims gRPC service,
/// mapping them into local PaymentOfClaims entities, validating and persisting them using the repository.
/// Also provides a read method to get payment status for a given month/year.
/// </summary>
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

    /// <summary>
    /// Calls the remote gRPC GetPaymentStatusReportsAsync to obtain payment data for the month/year.
    /// Behavior:
    /// - Validates input month/year; throws InvalidMonthOrYearException for invalid values.
    /// - Calls gRPC and inspects CommonOutputgRPC.StatusCode.
    /// - When StatusCode == Ok: TryUnpack the Any output into ClaimPaymentReportDTOgRPC and map to PaymentOfClaims.
    ///   - If a record for that month/year exists, update its Payment and persist via UpdatePaymentOfClaimsData.
    ///   - Otherwise generate a ReportId and insert via AddPaymentOfClaimsData.
    /// - When StatusCode == Badrequest: TryUnpack a StringValue containing the error message and return as failure.
    /// - Other status codes and unexpected states raise exceptions so the caller can handle them.
    /// </summary>
    public async Task<CommonOutput> AddPaymentOfClaimsStatus(int month,int year){

        CommonOutput result;
        try{

            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                // Input validation guard
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }

            // Call remote gRPC to fetch payment status report for month/year
            CommonOutputgRPC output=await _claimsService.GetPaymentStatusReportsAsync(new GetPaymentStatusReportsMonthAndYear{Month=month,Year=year});

            if(output.StatusCode==STATUSCODE.Ok){

                // The response Any is expected to contain a ClaimPaymentReportDTOgRPC instance.
                if(output.Output.TryUnpack(out ClaimPaymentReportDTOgRPC report)){
                    // Map gRPC DTO to local entity
                    PaymentOfClaims payment=_mapper.Map<PaymentOfClaims>(report);
                    
                    // Check repository for an existing record for the same month/year
                    PaymentOfClaims? alreadyPresent=await _paymentRepository.PaymentStatusOnMonthAndYear(month,year);

                    if(alreadyPresent!=null){
                        // Update existing record's Payment
                        alreadyPresent.Payment=payment.Payment;
                        result=await _paymentRepository.UpdatePaymentOfClaimsData(alreadyPresent);
                    }
                    else{
                        // New record: generate ReportId and add
                        payment.ReportId=GenerateReportId(payment);
                        result=await _paymentRepository.AddPaymentOfClaimsData(payment);
                    }

                    // Convert any ValidationResult collection in result.Output into PropertyValidationResponse list
                    GetErrorListInRequiredFormat(ref result);
                }
                else{
                    // Unexpected: Ok without expected payload
                    throw new Exception();
                }

            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                // gRPC returns a StringValue holding the error message
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
                // Propagate as exception so middleware/controller can map to 500
                throw new Exception();
            }
            else{
                result=new CommonOutput{Result=RESULT.FAILURE};
            }

        }
        catch(InvalidMonthOrYearException ex){
            // Return a failure CommonOutput containing the validation message
            return new CommonOutput{
                Result=RESULT.FAILURE,
                Output=ex.Message
            };
        }
        catch(Exception ex){
            // Re-throw to allow higher layers to handle/log. Consider logging here.
            throw;
        }
        return result;

    }

    /// <summary>
    /// Retrieves payment status for a given month/year from repository and returns a ClaimPaymentReportDTO.
    /// - Uses AsNoTracking on EF queries in repository to avoid tracking overhead for read-only data.
    /// </summary>
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


    // Generates a human-readable report id using prefix PS, 3-letter month and last two digits of year
    private static string GenerateReportId(PaymentOfClaims payment){
        return string.Concat("PS", payment.Month[..3].ToUpper(), payment.Year.ToString().Substring(2,2));
    }

    // Converts ValidationResult collection in CommonOutput.Output into a list of PropertyValidationResponse DTOs.
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
