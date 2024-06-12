namespace Surveyor.BLL;

public class ReportDTO
{
    public required string ClaimId{ get; set; }
    public required string PolicyNo{ get; set; }
    public int LabourCharges{ get; set; }
    public int PartsCost{ get; set; }
    public int PolicyClause{ get; set; }
    public int DepreciationCost{ get; set; }
    public int TotalAmount{ get; set; }
    public string? AccidentDetails{  get; set; }
}
