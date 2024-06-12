using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;

public interface IPolicyService
{
    Task<Policy?> GetPolicyByPolicyNo(string policyNo);
}
