using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

public interface IPaymentOfClaimsService
{
    Task<CommonOutput> AddPaymentOfClaimsStatus(int month,int year);

    Task<ClaimPaymentReportDTO> GetPaymentStatus(int month,int year);
}
