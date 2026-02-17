using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCClaimsService.Protos;
using gRPCSharedProtos.Protos;
using gRPCPoliciesService.Protos;
using SharedModules;

namespace Insured.BLL;

/// <summary>
/// Service layer for Insured operations. Uses a generated gRPC client to call the remote ClaimsService.
/// Responsibilities:
/// - Map local DTOs to gRPC request DTOs via AutoMapper.
/// - Call remote gRPC methods and interpret the CommonOutputgRPC responses.
/// - Unpack Any-typed outputs (e.g., StringValue, ErrorsListgRPC) using TryUnpack to obtain concrete types.
/// - Translate gRPC responses into local SharedModules.CommonOutput results.
/// </summary>
public class InsuredService:IInsuredService
{
    #pragma warning disable IDE0059 // Unnecessary assignment of a value
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly ClaimsService.ClaimsServiceClient _claimsclient;
    private readonly PoliciesService.PoliciesServiceClient _policyclient;
    private readonly IMapper _mapper;

    public InsuredService(ClaimsService.ClaimsServiceClient claimsclient, PoliciesService.PoliciesServiceClient policyclient, IMapper mapper)
    {
        _claimsclient = claimsclient;
        _policyclient = policyclient;
        _mapper = mapper;
    }

    /// <summary>
    /// Adds a new claim by calling the remote gRPC AddNewClaimAsync method.
    /// Behavior:
    /// - Maps local ClaimDetailRequestDTO to ClaimDetailRequestDTOgRPC.
    /// - Awaits the CommonOutputgRPC response which contains a StatusCode and an Any Output.
    /// - If StatusCode == Ok and the Any contains a StringValue, unpacks to get the claim id string.
    /// - If StatusCode == Badrequest and the Any contains ErrorsListgRPC, unpacks and maps each error into local PropertyValidationResponse.
    /// - Throws an exception for unexpected states; caller handles exceptions and converts to HTTP responses in controller.
    /// 
    /// Key helper explained:
    /// - output.Output.TryUnpack(out T typed) attempts to unpack a protobuf Any into the concrete message type T.
    ///   Returns true when the Any holds the expected type, false otherwise.
    /// </summary>
    public async Task<CommonOutput> AddNewClaim(ClaimDetailRequestDTO claim){

        CommonOutput result;

        try{
            // Map local DTO to gRPC DTO and call remote method.
            CommonOutputgRPC output=await _claimsclient.AddNewClaimAsync(_mapper.Map<ClaimDetailRequestDTOgRPC>(claim));

            if(output.StatusCode==STATUSCODE.Ok){
                // When Ok, output is expected to carry a Google.Protobuf.WellKnownTypes.StringValue with the claim id.
                if(output.Output.TryUnpack(out StringValue claimId)){
                    result=new CommonOutput{
                        Result=RESULT.SUCCESS,
                        Output=claimId.Value
                    };
                }
                else{
                    // Unexpected: Ok without expected payload
                    throw new Exception();
                }
            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                // When Badrequest, output is expected to carry validation errors (ErrorsListgRPC).
                if(output.Output.TryUnpack(out ErrorsListgRPC errs)){
                    // Build a local list of PropertyValidationResponse by mapping each gRPC error.
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
                    // Unexpected: Badrequest without expected error payload
                    throw new Exception();
                }
            }
            else{
                // Other status codes are not explicitly handled: treat as error.
                throw new Exception();
            }

        }
        catch(Exception ex){
            // Re-throw to allow higher layers (controller) to convert to HTTP error responses or logging.
            // In production, consider logging the exception and returning a controlled error object.
            throw;
        }

        return result;
    }

    /// <summary>
    /// Accepts or rejects a claim by calling the remote UpdateAcceptOrRejectClaimAsync gRPC method.
    /// Behavior:
    /// - Constructs an AcceptReject gRPC message with claimId and boolean flag.
    /// - Unpacks the response Any similarly to AddNewClaim for success or validation failure.
    /// - Returns CommonOutput with RESULT.SUCCESS or RESULT.FAILURE and mapped validation errors.
    /// </summary>
    public async Task<CommonOutput> AcceptOrRejectClaim(string claimId,AcceptRejectDTO acceptReject){

        CommonOutput result;
        try{
            // Call the remote method to update claim accept/reject status.
            CommonOutputgRPC output=await _claimsclient.UpdateAcceptOrRejectClaimAsync(new AcceptReject{ClaimId=claimId,IsAccept=acceptReject.AcceptReject});

            if(output.StatusCode==STATUSCODE.Ok){
                result=new CommonOutput{Result=RESULT.SUCCESS};
            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                // Unpack validation errors if present and map them.
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
            // Re-throw to allow controller to handle. Consider logging here.
            throw;
        }
        return result;
    }


    public async Task<CommonOutput> AddNewPolicy(string token,PolicyEntryDTO policy)
    {
        CommonOutput result;

        try
        {
            var headers=new Metadata
            {
                {"authorization",$"Bearer {token}"}  
            };
            // Map local DTO to gRPC DTO and call remote method.
            CommonOutputgRPC output = await _policyclient.AddNewPolicyAsync(_mapper.Map<PolicyRequestDTOgRPC>(policy),headers);

            if (output.StatusCode == STATUSCODE.Ok)
            {
                // When Ok, output is expected to carry a Google.Protobuf.WellKnownTypes.StringValue with the policy id.
                if (output.Output.TryUnpack(out StringValue policyId))
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.SUCCESS,
                        Output = policyId.Value
                    };
                }
                else
                {
                    // Unexpected: Ok without expected payload
                    throw new Exception();
                }
            }
            else if (output.StatusCode == STATUSCODE.Badrequest)
            {
                // When Badrequest, output is expected to carry validation errors (ErrorsListgRPC).
                if (output.Output.TryUnpack(out ErrorsListgRPC errs))
                {
                    // Build a local list of PropertyValidationResponse by mapping each gRPC error.
                    List<PropertyValidationResponse> errors = [];
                    foreach (var error in errs.Errors)
                    {
                        errors.Add(_mapper.Map<PropertyValidationResponse>(error));
                    }
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = errors
                    };
                }
                else
                {
                    // Unexpected: Badrequest without expected error payload
                    throw new Exception();
                }
            }
            else
            {
                // Other status codes are not explicitly handled: treat as error.
                throw new Exception();
            }

        }
        catch (Exception ex)
        {
            // Re-throw to allow higher layers (controller) to convert to HTTP error responses or logging.
            // In production, consider logging the exception and returning a controlled error object.
            throw;
        }

        return result;
    }

    public async Task<CommonOutput> GetPolicyByPolicyNumber(string token, string policyNumber)
    {
        CommonOutput result;

        try
        {
            var headers = new Metadata
            {
                { "authorization", $"Bearer {token}" }
            };

            
            // Call the remote method to get policy by policy number.
            CommonOutputgRPC output = await _policyclient.GetPolicyByPolicyNoAsync(new GetPolicyNoString { PolicyNo = policyNumber }, headers);

            if (output.StatusCode == STATUSCODE.Ok)
            {
                // When Ok, output is expected to carry a PolicyDetailDTOgRPC.
                if (output.Output.TryUnpack(out PolicyDTOgRPC policyDetail))
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.SUCCESS,
                        Output = _mapper.Map<PolicyResponseDTO>(policyDetail)
                    };
                }
                else
                {
                    // Unexpected: Ok without expected payload
                    throw new Exception();
                }
            }
            else if (output.StatusCode == STATUSCODE.Notfound)
            {
                result=new CommonOutput
                {
                  Result=RESULT.FAILURE,
                  Output=null  
                };
            }
            else if (output.StatusCode == STATUSCODE.Unauthorized)
            {
                if (output.Output.TryUnpack(out StringValue errStr))
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = errStr.Value
                    };
                }
                else
                {
                    // Unexpected: Ok without expected payload
                    throw new Exception();
                }
            }
            else
            {
                // Other status codes are not explicitly handled: treat as error.
                throw new Exception();
            }

        }
        catch (Exception ex)
        {
            // Re-throw to allow higher layers (controller) to convert to HTTP error responses or logging.
            // In production, consider logging the exception and returning a controlled error object.
            throw;
        }

        return result;
    }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
}
