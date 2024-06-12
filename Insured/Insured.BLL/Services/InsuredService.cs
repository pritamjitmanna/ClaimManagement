using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using SharedModules;

namespace Insured.BLL;

public class InsuredService:IInsuredService
{
    private readonly ClaimsService.ClaimsServiceClient _client;
    private readonly IMapper _mapper;

    public InsuredService(ClaimsService.ClaimsServiceClient client,IMapper mapper){
        _client = client;
        _mapper = mapper;
    }

    public async Task<CommonOutput> AddNewClaim(ClaimDetailRequestDTO claim){

        CommonOutput result;

        try{
            CommonOutputgRPC output=await _client.AddNewClaimAsync(_mapper.Map<ClaimDetailRequestDTOgRPC>(claim));

            if(output.StatusCode==STATUSCODE.Ok){
                if(output.Output.TryUnpack(out StringValue claimId)){
                    result=new CommonOutput{
                        Result=RESULT.SUCCESS,
                        Output=claimId.Value
                    };
                }
                else{
                    throw new Exception();
                }
            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                if(output.Output.TryUnpack(out ErrorsListgRPC errs)){
                    List<PropertyValidationResponse> errors=[];
                    foreach(var error in errs.Errors){
                        errors.Add(_mapper.Map<PropertyValidationResponse>(error));
                    }
                    result=new CommonOutput{
                        Result=RESULT.FAILURE,
                        Output=errors
                    };
                }
                else{
                    throw new Exception();
                }
            }
            else{
                throw new Exception();
            }

        }
        catch(Exception ex){
            throw;
        }

        return result;
    }

    public async Task<CommonOutput> AcceptOrRejectClaim(string claimId,AcceptRejectDTO acceptReject){

        CommonOutput result;
        try{
            CommonOutputgRPC output=await _client.UpdateAcceptOrRejectClaimAsync(new AcceptReject{ClaimId=claimId,IsAccept=acceptReject.AcceptReject});

            if(output.StatusCode==STATUSCODE.Ok){
                result=new CommonOutput{Result=RESULT.SUCCESS};
            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                if(output.Output.TryUnpack(out ErrorsListgRPC errs)){
                    List<PropertyValidationResponse> errors=[];
                    foreach(var error in errs.Errors){
                        errors.Add(_mapper.Map<PropertyValidationResponse>(error));
                    }
                    result=new CommonOutput{
                        Result=RESULT.FAILURE,
                        Output=errors
                    };
                }
                else{
                    throw new Exception();
                }
            }
            else{
                throw new Exception();
            }
        }
        catch(Exception ex){
            throw;
        }
        return result;
    }

}
