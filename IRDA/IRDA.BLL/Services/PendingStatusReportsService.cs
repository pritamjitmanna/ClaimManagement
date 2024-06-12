using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

public class PendingStatusReportsService:IPendingStatusReportsService
{
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    private readonly ClaimsService.ClaimsServiceClient _claimsService; 
    private readonly IPendingStatusReports _pendingStatusReports;
    private readonly IMapper _mapper;
    public PendingStatusReportsService(ClaimsService.ClaimsServiceClient claimsService,IPendingStatusReports pendingStatusReports,IMapper mapper){
        _claimsService = claimsService;
        _mapper = mapper;
        _pendingStatusReports=pendingStatusReports;
    }

    public async Task<CommonOutput> AddPendingStatusReports(int month,int year){

        CommonOutput result=new CommonOutput{
            Result=RESULT.SUCCESS
        };
        try{

            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }


            CommonOutputgRPC output=await _claimsService.GetClaimStatusReportsAsync(new GetClaimStatusReportsMonthAndYear{Month=month,Year=year});

            if(output.StatusCode==STATUSCODE.Ok){

                if(output.Output.TryUnpack(out ClaimStatusReportsListgRPC report)){
                    Console.WriteLine(report);
                    foreach(ClaimStatusReportDTOgRPC rep in report.Reports){
                        PendingStatusReports newVal=_mapper.Map<PendingStatusReports>(rep);
                        PendingStatusReports? alreadyExist=await _pendingStatusReports.PendingStatusReportsOnMonthAndYear(newVal.Stage,month,year);
                        CommonOutput temp;
                        if(alreadyExist!=null){
                            alreadyExist.Count=newVal.Count;
                            temp=await _pendingStatusReports.UpdatePendingStatusReportsData(alreadyExist);
                        }
                        else{
                            newVal.Month=MonthName[month-1];
                            newVal.Year=year;
                            newVal.ReportId=GenerateReportId(newVal);
                            temp=await _pendingStatusReports.AddPendingStatusReportsData(newVal);
                        }

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

    public async Task<IEnumerable<ClaimStatusReportDTO>> GetPendingStatusReports(int month,int year){
        
        List<ClaimStatusReportDTO> status=[];
        try
        {
            
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


    private static string GenerateReportId(PendingStatusReports status){
        return string.Concat("CS",status.Stage.ToString()[..1], status.Month[..3].ToUpper(), status.Year.ToString().Substring(2,2));
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
