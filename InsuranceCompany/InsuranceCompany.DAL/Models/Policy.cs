using System.ComponentModel.DataAnnotations;

namespace InsuranceCompany.DAL;

public class Policy
{
        public required string PolicyUserId { get; set; }
        [Length(7, 7, ErrorMessage = "PolicyNo should be of 7 characters")]
        public required string PolicyNo { get; set; }

        [MinLength(5, ErrorMessage = "InsuredFirstName should have atleast 5 characters")]
        public required string InsuredFirstName { get; set; }

        [MinLength(5, ErrorMessage = "InsuredLastName should have atleast 5 characters")]
        public required string InsuredLastName { get; set; }

        [DateGreaterEqualThanCurrent(ErrorMessage = "The DateOfInsurance must not be less than current date.")]
        public DateOnly DateOfInsurance { get; set; }
        public string? EmailId { get; set; }
        public required string VehicleNo { get; set; }
        public bool status { get; set; }

        public ICollection<ClaimDetail> ClaimDetails { get; set; }

}
