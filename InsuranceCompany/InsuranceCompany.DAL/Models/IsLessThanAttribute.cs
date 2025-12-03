using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
/// <summary>
/// Validation attribute that ensures an integer property value is less than
/// the Fee model's EstimateEndLimit. Intended for validating numeric range
/// against another property on the same model.
/// </summary>
public class IsLessThanAttribute:ValidationAttribute
{
    /// <summary>
    /// Validates that the provided value (expected to be an int) is less than
    /// the model's EstimateEndLimit. Returns ValidationResult.Success when valid,
    /// otherwise returns a ValidationResult containing the configured error message.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // If value not supplied, return a specific error here.
            if (value == null) return new ValidationResult("Value cannot be null");

            // Retrieve the whole object being validated (expected to be Fee).
            var model = (Fee)validationContext.ObjectInstance;

            // Compare the supplied value (cast to int) with the model's EstimateEndLimit.
            // If the value is strictly less, the validation succeeds.
            if ((int)value < model.EstimateEndLimit) return ValidationResult.Success;

            // Otherwise, produce a validation error with the attribute's ErrorMessage.
            return new ValidationResult(this.ErrorMessage);
        }
}
