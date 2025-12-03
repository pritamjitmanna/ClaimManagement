using System.ComponentModel.DataAnnotations;

namespace Surveyor.DAL;

/// <summary>
/// Validation helper using System.ComponentModel.DataAnnotations.
/// - TryValidateObject is used to collect ValidationResult entries for the object graph.
/// - Returns true if the model is valid; results contains the detected validation errors otherwise.
/// </summary>
public class ValidationFunctions
{
    public static bool ValidateModel(SurveyReport surveyReport,ref ICollection<ValidationResult> results){
        ValidationContext vc=new(surveyReport);
        bool IsValid=Validator.TryValidateObject(surveyReport,vc,results,true);
        return IsValid;
    }
}
