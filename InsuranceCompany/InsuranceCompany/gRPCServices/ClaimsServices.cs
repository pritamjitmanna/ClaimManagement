namespace InsuranceCompany;

using Grpc.Core;
using gRPCClaimsService.Protos;
using InsuranceCompany.BLL;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using InsuranceCompany.DAL;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SharedModules;

public class ClaimsServices:ClaimsService.ClaimsServiceBase
{
    #pragma warning disable IDE0059 // Unnecessary assignment of a value
    #pragma warning disable CS0168 // Variable is declared but never used
    #pragma warning disable CS8602 //Dereference of a possibly null reference.
    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    private readonly ISharedLogic _sharedLogic;
    private readonly IClaimDetailService _claimService;
    private readonly IPolicyService _policyService;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;
    const string INTERNAL_SERVER_ERROR = "There's an unexpected Internal error. Sorry for the inconvenience caused. Please try again after some time";

    public ClaimsServices(ISharedLogic sharedLogic,IClaimDetailService claimDetail,IPolicyService policyService, IMapper mapper)
    {
        _sharedLogic = sharedLogic;
        _claimService = claimDetail;
        _policyService = policyService;
        _mapper = mapper;
        //_logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    public async override Task<CommonOutputgRPC> AddNewClaim(ClaimDetailRequestDTOgRPC request,ServerCallContext context){

        try{
            ClaimDetailRequestDTO claimDetail=_mapper.Map<ClaimDetailRequestDTO>(request);
            CommonOutput result=await _sharedLogic.AddClaimSharedLogic(claimDetail);
            if(result.Result==RESULT.SUCCESS){
                return await Task.FromResult(new CommonOutputgRPC{
                    StatusCode=STATUSCODE.Ok,
                    Output=Any.Pack(new StringValue{Value=result.Output.ToString()})
                });
            }
            ErrorsListgRPC errors=new();
            List<PropertyValidationResponse>? errs=(List<PropertyValidationResponse>?)result.Output;
            List<PropertyValidationResponsegRPC> grpcErrs=[];
            foreach(var err in errs){
                grpcErrs.Add(_mapper.Map<PropertyValidationResponsegRPC>(err));
            }
            errors.Errors.AddRange(grpcErrs);
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(errors),
                StatusCode=STATUSCODE.Badrequest
            });
        }
        catch(Exception ex){
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }

    }


    public async override Task<CommonOutputgRPC> GetClaimByClaimId(GetClaimByIdString request,ServerCallContext context)
    {
        try
        {
            CommonOutput output=await _sharedLogic.GetClaimByClaimId(request.ClaimId);
            Console.WriteLine(output);
            if (output.Result==RESULT.SUCCESS)
            {

                return await Task.FromResult(new CommonOutputgRPC{
                    Output=Any.Pack(_mapper.Map<ClaimDTOgRPC>((ClaimListOpenDTO)output.Output)),
                    StatusCode=STATUSCODE.Ok
                });
            }
            return await Task.FromResult(new CommonOutputgRPC{
                StatusCode=STATUSCODE.Notfound
            });

        }
        catch(Exception ex)
        {
            //_logger.Error(LogMessage(   ex.Message));
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }
    }

    public async override Task<CommonOutputgRPC> GetPolicyByPolicyNo(GetPolicyNoString request,ServerCallContext context){
        try
        {
            Policy? output=await _policyService.GetPolicyByPolicyNo(request.PolicyNo);
            if (output!=null)
            {
                return await Task.FromResult(new CommonOutputgRPC{
                    Output=Any.Pack(_mapper.Map<PolicyDTOgRPC>(output)),
                    StatusCode=STATUSCODE.Ok
                });
            }
            return await Task.FromResult(new CommonOutputgRPC{
                StatusCode=STATUSCODE.Notfound
            });

        }
        catch(Exception ex)
        {
            //_logger.Error(LogMessage(   ex.Message));
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }
    }



    public async override Task<CommonOutputgRPC> GetClaimStatusReports(GetClaimStatusReportsMonthAndYear request,ServerCallContext context)
    {
        try
        {

            CommonOutput output= await _sharedLogic.GetClaimStatusReports(request.Month,request.Year);
            if(output.Result==RESULT.SUCCESS){
                ClaimStatusReportsListgRPC reports=new();

                List<ClaimStatusReportDTOgRPC> reps=[];
                foreach(var rep in (IEnumerable<ClaimStatusReportDTO>?)output.Output){
                    reps.Add(_mapper.Map<ClaimStatusReportDTOgRPC>(rep));
                }
                reports.Reports.AddRange(reps);
                return await Task.FromResult(new CommonOutputgRPC{
                    Output=Any.Pack(reports),
                    StatusCode=STATUSCODE.Ok
                });
            }
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=(string?)output.Output}),
                StatusCode=STATUSCODE.Badrequest
            });
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }
    }


    public async override Task<CommonOutputgRPC> GetPaymentStatusReports(GetPaymentStatusReportsMonthAndYear request,ServerCallContext context){

        try{
            CommonOutput output=await _sharedLogic.GetPaymentStatusReports(request.Month,request.Year);

            if(output.Result==RESULT.SUCCESS){
                return await Task.FromResult(new CommonOutputgRPC{
                    Output=Any.Pack(_mapper.Map<ClaimPaymentReportDTOgRPC>(output.Output)),
                    StatusCode=STATUSCODE.Ok
                });
            }
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=(string?)output.Output}),
                StatusCode=STATUSCODE.Badrequest
            });
        }
        catch (Exception ex){
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }

    }

    public async override Task<CommonOutputgRPC> UpdateClaimAmountApprovedBySurveyor(UpdateClaimAmountApprovedBySurveyorClaimIdClaimant request,ServerCallContext context){

        try{
            CommonOutput result = await _sharedLogic.UpdateClaimAmountApprovedBySurveyor(request.ClaimID,request.Claimant);
            if(result.Result==RESULT.SUCCESS){
                return await Task.FromResult(new CommonOutputgRPC{
                    StatusCode=STATUSCODE.Ok
                });
            }
            ErrorsListgRPC errors=new();
            List<PropertyValidationResponse>? errs=(List<PropertyValidationResponse>?)result.Output;
            List<PropertyValidationResponsegRPC> grpcErrs=[];
            foreach(var err in errs){
                grpcErrs.Add(_mapper.Map<PropertyValidationResponsegRPC>(err));
            }
            errors.Errors.AddRange(grpcErrs);
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(errors),
                StatusCode=STATUSCODE.Badrequest
            });
        }
        catch (Exception ex){
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }
    } 

    public async override Task<CommonOutputgRPC> UpdateAcceptOrRejectClaim(AcceptReject acceptReject,ServerCallContext context){

        try{
            CommonOutput output=await _claimService.UpdateAcceptRejectClaim(acceptReject.ClaimId,acceptReject.IsAccept);
            if(output.Result==RESULT.SUCCESS){
                return await Task.FromResult(new CommonOutputgRPC{
                    StatusCode=STATUSCODE.Ok
                });
            }
            ErrorsListgRPC errors=new();
            List<PropertyValidationResponse>? errs=(List<PropertyValidationResponse>?)output.Output;
            List<PropertyValidationResponsegRPC> grpcErrs=[];
            foreach(var err in errs){
                grpcErrs.Add(_mapper.Map<PropertyValidationResponsegRPC>(err));
            }
            errors.Errors.AddRange(grpcErrs);
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(errors),
                StatusCode=STATUSCODE.Badrequest
            });
        }
        catch(Exception ex){
            return await Task.FromResult(new CommonOutputgRPC{
                Output=Any.Pack(new StringValue{Value=INTERNAL_SERVER_ERROR}),
                StatusCode=STATUSCODE.Internalservererror
            });
        }

    }

    
}
