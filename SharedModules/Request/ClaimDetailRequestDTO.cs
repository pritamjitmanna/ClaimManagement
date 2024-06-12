namespace SharedModules;

public class ClaimDetailRequestDTO
{
    public string? PolicyNo { get; set; }
    public int? EstimatedLoss { get; set; }
    public DateOnly? DateOfAccident { get; set; }
}
