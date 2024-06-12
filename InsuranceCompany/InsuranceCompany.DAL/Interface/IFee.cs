namespace InsuranceCompany.DAL;

/// <summary>
/// This interface is used to build the Fee Repository for operations on the Fee table.
/// </summary>
public interface IFee
{

    /// <summary>
    /// This function returns the Fee object whose estimated limit range contains the provided the estimatedLoss value.
    /// </summary>
    /// <param name="estimatedLoss"></param>
    /// <returns>Fee</returns>
    Task<Fee?> GetFeesByEstimatedLoss(int estimatedLoss);
}
