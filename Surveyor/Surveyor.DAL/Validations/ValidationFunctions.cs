using System.ComponentModel.DataAnnotations;

namespace Surveyor.DAL;

public class ValidationFunctions
{
    public static bool ValidateModel(SurveyReport surveyReport,ref ICollection<ValidationResult> results){
        ValidationContext vc=new(surveyReport);
        bool IsValid=Validator.TryValidateObject(surveyReport,vc,results,true);
        return IsValid;
    }
}
