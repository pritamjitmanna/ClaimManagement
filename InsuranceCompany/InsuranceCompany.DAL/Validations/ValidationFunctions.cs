using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

/// <summary>
/// This class is a validation class which contains static functions for validating the claimDetail model for the validation errors.
/// </summary>
public class ValidationFunctions
{

    /// <summary>
    /// This function validates the claimDetail object for the validation errors. It uses the validation context for checking and creating the error lists.
    /// The validation errors are returned in a reference object.
    /// </summary>
    /// <param name="claimDetail"></param>
    /// <param name="results"></param>
    /// <returns>bool</returns>
    public static bool ValidateModel(ClaimDetail claimDetail, ref ICollection<ValidationResult> results)
    {
        ValidationContext vc = new ValidationContext(claimDetail);
        bool IsValid = Validator.TryValidateObject(claimDetail, vc, results, true);
        return IsValid;
    }

    public static bool ValidateModel(Surveyor surveyor, ref ICollection<ValidationResult> results)
    {
        ValidationContext vc = new ValidationContext(surveyor);
        bool IsValid = Validator.TryValidateObject(surveyor, vc, results, true);
        return IsValid;
    }

    public static bool ValidateModel(Policy policy, ref ICollection<ValidationResult> results)
    {
        ValidationContext vc = new ValidationContext(policy);
        bool IsValid = Validator.TryValidateObject(policy, vc, results, true);
        return IsValid;
    }
}
