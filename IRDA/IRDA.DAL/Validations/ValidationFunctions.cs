using System.ComponentModel.DataAnnotations;

namespace IRDA.DAL;

public class ValidationFunctions
{

    public static bool ValidateModel(PaymentOfClaims payment,ref ICollection<ValidationResult> results){
        ValidationContext vc=new(payment);
        bool IsValid=Validator.TryValidateObject(payment,vc,results,true);
        return IsValid;
    }

    public static bool ValidateModel(PendingStatusReports payment,ref ICollection<ValidationResult> results){
        ValidationContext vc=new(payment);
        bool IsValid=Validator.TryValidateObject(payment,vc,results,true);
        return IsValid;
    }
}
