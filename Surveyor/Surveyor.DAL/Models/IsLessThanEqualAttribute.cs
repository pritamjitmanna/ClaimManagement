using System.ComponentModel.DataAnnotations;

namespace Surveyor.DAL;

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
