namespace InsuranceCompany.DAL;

public class Fee
{
    public int FeeId { get; set; }
        [IsLessThan(ErrorMessage = "EstimateStartLimit must be less than EstimatedEndLimit")]
        public int EstimateStartLimit { get; set; }
        public int EstimateEndLimit { get; set; }
        public int fees { get; set; }
}
