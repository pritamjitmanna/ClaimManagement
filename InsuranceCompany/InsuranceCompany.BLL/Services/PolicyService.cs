// Summary:
// PolicyService is a thin service that delegates policy retrieval to IPolicy repository.
// It uses async/await and forwards exceptions. The service shields higher layers from direct DAL access.

using System.ComponentModel.DataAnnotations;
using AutoMapper;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class PolicyService : IPolicyService
{
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
    private readonly IPolicy _policyRepository;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;

    public PolicyService(IPolicy policyRepository, IMapper mapper)
    {
        _policyRepository = policyRepository;
        _mapper = mapper;
        //_logger = logger;
    }

    // GetPolicyByPolicyNo:
    // - Delegates to repository GetPolicyByPolicyNo (which uses EF Include and AsNoTracking).
    // - Uses await to asynchronously obtain result and returns Policy or null.
    public async Task<CommonOutput> GetPolicyByPolicyNo(string userId,string policyNo)
    {

        CommonOutput result;


        try
        {
            Policy? policy = await _policyRepository.GetPolicyByPolicyNo(policyNo);
            if (policy == null)
            {
                result=new CommonOutput
                {
                    Result=RESULT.FAILURE,
                    Output=null
                };
            }
            else if(policy.PolicyUserId != userId)     //--- To change when claim userId is added
            {
                result=new CommonOutput
                {
                    Result=RESULT.FAILURE,
                    Output="Unauthorized access to the policy."
                };
            }
            else
            {
                result=new CommonOutput
                {
                    Result=RESULT.SUCCESS,
                    Output=policy
                };
            }
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in PolicyService");
            throw;
        }

        return result;
    }

    public async Task<CommonOutput> AddNewPolicy(string userId, PolicyEntryDTO policy)
    {
        CommonOutput result;
        try
        {
            Policy newPolicy = _mapper.Map<Policy>(policy);
            newPolicy.PolicyUserId = userId;
            newPolicy.PolicyNo = GeneratePolicyNumber(newPolicy);
            result = await _policyRepository.AddNewPolicy(newPolicy);

            GetErrorListInRequiredFormat(ref result);
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in PolicyService");
            throw;
        }
        return result;
    }


    // ------------- Helpers -------------

    private string GeneratePolicyNumber(Policy policy)
    {
        string policyNo=policy.InsuredLastName.Substring(0,2).ToUpper();
        policyNo+=policy.VehicleNo.Substring(policy.VehicleNo.Length - 4,3);
        policyNo+=DateTime.Now.Year.ToString().Substring(2,2);
        return policyNo;
    }


    private void GetErrorListInRequiredFormat(ref CommonOutput result)
    {
        if (result.Result == RESULT.FAILURE)
        {
            List<PropertyValidationResponse> validationErrors = new List<PropertyValidationResponse>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var err in (ICollection<ValidationResult>?)result.Output)
            {
                validationErrors.Add(
                    new PropertyValidationResponse
                    {
                        Property = err.MemberNames.First(),
                        ErrorMessage = err.ErrorMessage
                    });
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            result.Output = validationErrors;
        }
    }

}

