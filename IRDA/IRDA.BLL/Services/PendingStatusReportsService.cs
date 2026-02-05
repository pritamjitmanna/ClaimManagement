using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using gRPCSharedProtos.Protos;
using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

/// <summary>
/// Service that pulls claim status reports from the remote Claims gRPC service and maintains PendingStatusReports locally.
/// - Adds or updates multiple PendingStatusReports entries for different stages based on the fetched reports.
/// - Provides a method to return aggregated status counts for all Stages for a month/year.
/// </summary>
public class PendingStatusReportsService:IPendingStatusReportsService
{
    // MonthName array used to convert numeric month into a string month name when persisting records.
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    private readonly ClaimsService.ClaimsServiceClient _claimsService; 
    private readonly IPendingStatusReports _pendingStatusReports;
    private readonly IMapper _mapper;
    public PendingStatusReportsService(ClaimsService.ClaimsServiceClient claimsService,IPendingStatusReports pendingStatusReports,IMapper mapper){
        _claimsService = claimsService;
        _mapper = mapper;
        _pendingStatusReports=pendingStatusReports;
    }

    /// <summary>
    /// Calls remote GetClaimStatusReportsAsync and processes the returned list of reports.
    /// For each report:
    /// - Maps gRPC DTO to PendingStatusReports entity.
    /// - If a matching record exists for the same stage/month/year, updates it; otherwise inserts a new record with generated ReportId.
    /// - On any repository validation failure, stops processing and returns the failure result.
    /// </summary>
    public async Task<CommonOutput> AddPendingStatusReports(int month,int year){

        CommonOutput result=new CommonOutput{
            Result=RESULT.SUCCESS
        };
        try{

            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }

            // Call gRPC to get claim status reports list for the month/year
            CommonOutputgRPC output=await _claimsService.GetClaimStatusReportsAsync(new GetClaimStatusReportsMonthAndYear{Month=month,Year=year});

            if(output.StatusCode==STATUSCODE.Ok){

                // Expect the Any output to be a ClaimStatusReportsListgRPC which contains multiple ClaimStatusReportDTOgRPC items
                if(output.Output.TryUnpack(out ClaimStatusReportsListgRPC report)){
                    Console.WriteLine(report);
                    foreach(ClaimStatusReportDTOgRPC rep in report.Reports){
                        // Map each gRPC report to local PendingStatusReports entity
                        PendingStatusReports newVal=_mapper.Map<PendingStatusReports>(rep);
                        // Check if an entry for this stage/month/year already exists
                        PendingStatusReports? alreadyExist=await _pendingStatusReports.PendingStatusReportsOnMonthAndYear(newVal.Stage,month,year);
                        CommonOutput temp;
                        if(alreadyExist!=null){
                            // Update existing record count
                            alreadyExist.Count=newVal.Count;
                            temp=await _pendingStatusReports.UpdatePendingStatusReportsData(alreadyExist);
                        }
                        else{
                            // Prepare new record (set Month name, Year and generated ReportId) and add it
                            newVal.Month=MonthName[month-1];
                            newVal.Year=year;
                            newVal.ReportId=GenerateReportId(newVal);
                            temp=await _pendingStatusReports.AddPendingStatusReportsData(newVal);
                        }

                        // If the repo returned a failure (e.g., validation errors), break and propagate it
                        if(temp.Result==RESULT.FAILURE){
                            result=temp;
                            break;
                        }
                    }
                    if(result.Result==RESULT.FAILURE){
                        GetErrorListInRequiredFormat(ref result);
                    }
                }
                else{
                    throw new Exception();
                }

            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                // Remote service returned a bad request; unpack expected string message
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
            // Return a controlled failure result for invalid inputs
            return new CommonOutput{
                Result=RESULT.FAILURE,
                Output=ex.Message
            };
        }
        catch(Exception ex){
            // Re-throw so controller/middleware can handle & log
            throw;
        }
        return result;

    }

    /// <summary>
    /// Builds a list of ClaimStatusReportDTO entries containing counts for each Stage for the given month/year.
    /// - Iterates all values of the Stages enum and queries repository for each stage's record.
    /// - Uses 0 when no record is found.
    /// </summary>
    public async Task<IEnumerable<ClaimStatusReportDTO>> GetPendingStatusReports(int month,int year){
        
        List<ClaimStatusReportDTO> status=[];
        try
        {
            // Iterate all enum values for Stages and query repository for each
            foreach (Stages e in System.Enum.GetValues(typeof(Stages)))
            {
                PendingStatusReports? temp=await _pendingStatusReports.PendingStatusReportsOnMonthAndYear(e,month,year);
                status.Add(new ClaimStatusReportDTO
                {
                    Stage = e,
                    Count = temp==null?0:temp.Count
                });
            }
        }
        catch (Exception ex)
        {
            //Log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return status;
    }


    // Generates a report id using prefix CS, first letter of Stage, 3-letter month and last two digits of year
    private static string GenerateReportId(PendingStatusReports status){
        return string.Concat("CS",status.Stage.ToString()[..1], status.Month[..3].ToUpper(), status.Year.ToString().Substring(2,2));
    }

    // Converts ValidationResult collections returned by repository into a list of PropertyValidationResponse items
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
