using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

///<summary>
/// Validation attribute that ensures a DateOnly? value (e.g., DateOfAccident) is strictly
/// less than the current date. This is useful for validating past-only dates like accident dates.
/// If the date is today or in the future, validation fails.
///</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateLessThanCurrentAttribute : ValidationAttribute
{

    /// <summary>
    /// Casts the incoming value to a nullable DateOnly and checks if it is earlier than today.
    /// Returns Success when the date is in the past; otherwise returns a ValidationResult referencing
    /// the "DateOfAccident" member name so UI/model binding can display the error appropriately.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Nullable cast allows for handling nulls explicitly in the logic below.
        var val = (DateOnly?)value;

        // If the date is strictly less than today, validation succeeds.
        if (val < DateOnly.FromDateTime(DateTime.Today)) return ValidationResult.Success;

        // Otherwise, produce a validation error targeted at the DateOfAccident field.
        return new ValidationResult(this.ErrorMessage, new[] { "DateOfAccident" });
    }
}
