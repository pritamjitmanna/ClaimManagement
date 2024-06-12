using System.ComponentModel.DataAnnotations;

namespace IRDA.DAL;

public class PaymentOfClaims
{
    public string ReportId { get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "Payment cannot be negative")]
    public int Payment{ get; set; }
    public required string Month{ get; set; }
    public int Year{ get; set; }

}
