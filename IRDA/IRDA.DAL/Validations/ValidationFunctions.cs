using System.ComponentModel.DataAnnotations;

namespace IRDA.DAL;

/// <summary>
/// Helper class providing DataAnnotations-based validation functions for domain entities.
/// - Uses Validator.TryValidateObject to execute attribute-based validation and collect ValidationResult entries.
/// - Methods return true when model is valid; results contains validation errors when false.
/// </summary>
public class ValidationFunctions
{

    public static bool ValidateModel(PaymentOfClaims payment,ref ICollection<ValidationResult> results){
        // Build a ValidationContext for the object and perform validation. 'true' enables recursive validation of properties.
        ValidationContext vc=new(payment);
        bool IsValid=Validator.TryValidateObject(payment,vc,results,true);
        return IsValid;
    }

    public static bool ValidateModel(PendingStatusReports payment,ref ICollection<ValidationResult> results){
        // Same approach for PendingStatusReports entity
        ValidationContext vc=new(payment);
        bool IsValid=Validator.TryValidateObject(payment,vc,results,true);
        return IsValid;
    }
}
