using System.ComponentModel.DataAnnotations;

namespace Surveyor.DAL;

public class SurveyReport
{
    public required string ClaimId{ get; set; }
    public required string PolicyNo{ get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "LabourCharges cannot be negative")]
    [IsLessThanEqual("PartsCost",ErrorMessage="LabourCharges cannot be greater than PartsCost")]
    public int LabourCharges{ get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "PartsCost cannot be negative")]
    public int PartsCost{ get; set; }
    [Range(1, int.MaxValue,ErrorMessage = "PolicyClause should be greater than 0")]
    public int PolicyClause{ get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "DepreciationCost cannot be negative")]
    public int DepreciationCost{ get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "TotalAmount cannot be negative")]
    [IsLessThanEqual("EstimatedLoss",ErrorMessage ="TotalAmount should not be greater than the Estimated Loss. Modify PartsCost or LabourCharges to lower the Amount")]
    public int TotalAmount{ get; set; }
    [MinLength(200,ErrorMessage = "AccidentDetails should be atleast 200 characters long")]
    public string? AccidentDetails{  get; set; }


    //Other details
    public int VehicleAge{ get; set; }
    public int EstimatedLoss{ get; set; }

}
