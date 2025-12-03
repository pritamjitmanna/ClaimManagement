using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
/// <summary>
/// Validation attribute that ensures a DateOnly value is greater than or equal to
/// the current date (today). Used to validate future-or-today date requirements
/// such as the start date of an insurance period.
/// </summary>
public class DateGreaterEqualThanCurrentAttribute : ValidationAttribute
{

    /// <summary>
    /// Casts the incoming value to DateOnly and compares it against today's date.
    /// Returns Success if date is today or in the future; otherwise returns a ValidationResult
    /// that points to the "DateOfInsurance" member name.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Expecting a DateOnly value; perform direct comparison with today's date.
        var val = (DateOnly)value;

        // If the provided date is today or later, validation succeeds.
        if (val >= DateOnly.FromDateTime(DateTime.Today)) return ValidationResult.Success;

        // If validation fails, include the related member name to help model binding display the error.
        return new ValidationResult(this.ErrorMessage, new[] { "DateOfInsurance" });
    }


}
