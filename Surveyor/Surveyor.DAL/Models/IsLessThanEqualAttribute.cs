using System.ComponentModel.DataAnnotations;

namespace Surveyor.DAL;

/// <summary>
/// Custom ValidationAttribute that enforces the decorated integer property is less than or equal to another property on the same object.
/// - The constructor accepts the name of the other property to compare against.
/// - IsValid performs reflection to obtain the comparison property value and returns ValidationResult.Success when the condition holds.
/// - Returns useful error messages when values or properties are null or the condition fails.
/// </summary>
public class IsLessThanEqualAttribute:ValidationAttribute
{

    readonly string _prop;

    public IsLessThanEqualAttribute(string prop){
        _prop = prop;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)return new ValidationResult("Value cannot be null");
        var property=validationContext.ObjectInstance.GetType().GetProperty(this._prop);
        if(property == null)return new ValidationResult($"{this._prop} cannot be null");
        var propertyValue=property.GetValue(validationContext.ObjectInstance,null);
        if(propertyValue == null)return new ValidationResult($"{this._prop} cannot be null");
        // Console.WriteLine(propertyValue);
        if((int)value<=(int)propertyValue)return ValidationResult.Success;
        return new ValidationResult(this.ErrorMessage,[validationContext.DisplayName] ); 
    }

}
