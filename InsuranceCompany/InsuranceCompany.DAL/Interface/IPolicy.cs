namespace InsuranceCompany.DAL;

/// <summary>
/// This interface deals with the Policy Table operations.
/// </summary>
public interface IPolicy
{
    /// <summary>
    /// It retrives the Policy object by the given policyNo. It may return NULL.
    /// </summary>
    /// <param name="policyNo"></param>
    /// <returns>Policy?</returns>
    Task<Policy?> GetPolicyByPolicyNo(string policyNo);
}
