
namespace SharedModules;
public class PolicyResponseDTO
{
    public string PolicyNo{get; set;}
    public string InsuredFirstName{get; set;} 
    public string InsuredLastName{get; set;}
    public DateOnly DateOfInsurance{get; set;} 
    public string VehicleNo{get; set;}
    public bool Status{get; set;} 
}
