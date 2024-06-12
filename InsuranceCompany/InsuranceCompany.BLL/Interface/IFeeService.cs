namespace InsuranceCompany.BLL;

public interface IFeeService
{
    Task<FeeDTO?> GetFeesByEstimatedLoss(int estimatedLoss);
}
