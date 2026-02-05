using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public interface IPolicyService
{
    Task<CommonOutput> GetPolicyByPolicyNo(string userId,string policyNo);
    Task<CommonOutput> AddNewPolicy(string userId, PolicyEntryDTO policy);
}
