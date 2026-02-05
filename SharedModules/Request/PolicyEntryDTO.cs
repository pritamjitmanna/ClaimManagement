using System;

namespace SharedModules;

public class PolicyEntryDTO
{
    public string InsuredFirstName { get; set; }
    public string InsuredLastName { get; set; }
    public DateOnly DateOfInsurance { get; set; }
    public string VehicleNo { get; set; }
}
