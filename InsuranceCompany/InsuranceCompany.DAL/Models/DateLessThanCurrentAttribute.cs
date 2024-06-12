using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

///<summary>
///It checks if the DateOfAccident is less than the current date. It shows validation error if it is greater or equal to the current date.
///</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateLessThanCurrentAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var val = (DateOnly?)value;
        if (val < DateOnly.FromDateTime(DateTime.Today)) return ValidationResult.Success;
        return new ValidationResult(this.ErrorMessage, new[] { "DateOfAccident" });
    }
}
