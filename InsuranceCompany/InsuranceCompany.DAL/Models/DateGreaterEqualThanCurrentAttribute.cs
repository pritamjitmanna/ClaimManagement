using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateGreaterEqualThanCurrentAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var val = (DateOnly)value;
        if (val >= DateOnly.FromDateTime(DateTime.Today)) return ValidationResult.Success;
        return new ValidationResult(this.ErrorMessage, new[] { "DateOfInsurance" });
    }


}
