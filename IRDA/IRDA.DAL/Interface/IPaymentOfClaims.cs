using SharedModules;

namespace IRDA.DAL;

public interface IPaymentOfClaims
{
    Task<PaymentOfClaims?> PaymentStatusOnMonthAndYear(int month, int year);
    Task<CommonOutput> AddPaymentOfClaimsData(PaymentOfClaims payment);
    Task<CommonOutput> UpdatePaymentOfClaimsData(PaymentOfClaims payment);
}
