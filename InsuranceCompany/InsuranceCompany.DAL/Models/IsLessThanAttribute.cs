using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class IsLessThanAttribute:ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("Value cannot be null");
            var model = (Fee)validationContext.ObjectInstance;
            if ((int)value < model.EstimateEndLimit) return ValidationResult.Success;
            return new ValidationResult(this.ErrorMessage);
        }
}
