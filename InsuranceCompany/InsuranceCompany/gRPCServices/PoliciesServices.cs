using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCPoliciesService.Protos;
using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using SharedModules;
using gRPCSharedProtos.Protos;

namespace InsuranceCompany.gRPCServices
{
    public class PoliciesServices:PoliciesService.PoliciesServiceBase
    {
        #pragma warning disable CS8604 // Possible null reference argument.
        #pragma warning disable CS0168 // Variable is declared but never used
        #pragma warning disable CS8602 //Dereference of a possibly null reference.
        private readonly IPolicyService _policyService;
        private readonly IMapper _mapper;

        const string INTERNAL_SERVER_ERROR = "There's an unexpected Internal error. Sorry for the inconvenience caused. Please try again after some time";
        
        public PoliciesServices(IPolicyService policyService, IMapper mapper)
        {
            _policyService = policyService;
            _mapper = mapper;
        }


        // GetPolicyByPolicyNo:
    // - Delegates to PolicyService; maps Policy to PolicyDTOgRPC on success.
    // - Returns STATUSCODE.Notfound when policy doesn't exist.
        public async override Task<CommonOutputgRPC> GetPolicyByPolicyNo(GetPolicyNoString request,ServerCallContext context){
            try
            {
                var userId=context.GetHttpContext().User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                CommonOutput result=await _policyService.GetPolicyByPolicyNo(userId,request.PolicyNo);

                if (result.Result == RESULT.SUCCESS)
                {
                    return await Task.FromResult(new CommonOutputgRPC{
                        Output=Any.Pack(_mapper.Map<PolicyDTOgRPC>(result.Output)),
                        StatusCode=STATUSCODE.Ok
                    });
                }
                else if (result.Output == null)
                {
                    return await Task.FromResult(new CommonOutputgRPC{
                        StatusCode=STATUSCODE.Notfound
                    });
                }
                else
                {
                    return await Task.FromResult(new CommonOutputgRPC{
                        Output=Any.Pack(new StringValue{Value=result.Output.ToString()}),
                        StatusCode=STATUSCODE.Unauthorized
                    });
                }

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

        public async override Task<CommonOutputgRPC> AddNewPolicy(PolicyRequestDTOgRPC policyRequestDTOgRPC, ServerCallContext context)
        {
            try
            {
                var userId=context.GetHttpContext().User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                PolicyEntryDTO policyEntryDTO = _mapper.Map<PolicyEntryDTO>(policyRequestDTOgRPC);
                CommonOutput result = await _policyService.AddNewPolicy(userId, policyEntryDTO);
                if (result.Result == RESULT.SUCCESS)
                {
                    return await Task.FromResult(new CommonOutputgRPC
                    {
                        StatusCode = STATUSCODE.Ok,
                        Output = Any.Pack(new StringValue { Value = result.Output.ToString() })
                    });
                }
                ErrorsListgRPC errors = new();
                List<PropertyValidationResponse>? errs = (List<PropertyValidationResponse>?)result.Output;
                List<PropertyValidationResponsegRPC> grpcErrs = [];
                foreach (var err in errs)
                {
                    grpcErrs.Add(_mapper.Map<PropertyValidationResponsegRPC>(err));
                }
                errors.Errors.AddRange(grpcErrs);
                return await Task.FromResult(new CommonOutputgRPC
                {
                    Output = Any.Pack(errors),
                    StatusCode = STATUSCODE.Badrequest
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new CommonOutputgRPC
                {
                    Output = Any.Pack(new StringValue { Value = INTERNAL_SERVER_ERROR }),
                    StatusCode = STATUSCODE.Internalservererror
                });
            }
        }
        #pragma warning restore CS8604 // Possible null reference argument.
        #pragma warning restore CS0168 // Variable is declared but never used
        #pragma warning restore CS8602 //Dereference of a possibly null reference.
    
    }
    
}