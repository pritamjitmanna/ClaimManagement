// Summary:
// SharedLogic is an application-level layer that coordinates simple validation and delegates
// business operations to the ClaimDetailService (IClaimDetailService). It centralizes
// basic input validation and maps service outcomes to CommonOutput responses suitable for APIs.
// Key behaviors:
// - Performs lightweight input checks (null/required fields).
using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany;

public class SharedLogic:ISharedLogic
{
    #pragma warning disable IDE0059 // Unnecessary assignment of a value
    #pragma warning disable CS0168 // Variable is declared but never used
    private readonly IClaimDetailService _claimDetailService;
    public SharedLogic(IClaimDetailService claimDetailService){
        _claimDetailService = claimDetailService;
    }

    // AddClaimSharedLogic:
    // - Performs basic validation for required fields (PolicyNo, EstimatedLoss, DateOfAccident).
    // - If validation passes, delegates to ClaimDetailService.AddNewClaim and returns its CommonOutput.
    // - Catches domain-specific MaximumClaimLimitReachedException and converts it to a CommonOutput failure.
    public async Task<CommonOutput> AddClaimSharedLogic(ClaimDetailRequestDTO claimDetail){
        try
        {
            List<PropertyValidationResponse> errors = new List<PropertyValidationResponse>();

            if (claimDetail.PolicyNo == null)
            {
                errors.Add(new PropertyValidationResponse { Property = "PolicyNo", ErrorMessage = "PolicyId not provided" });
            }

            if (claimDetail.EstimatedLoss == null)
            {
                errors.Add(new PropertyValidationResponse { Property = "EstimatedLoss", ErrorMessage = "EstimatedLoss must be provided" });
            }

            if (claimDetail.DateOfAccident == null)
            {
                errors.Add(new PropertyValidationResponse { Property = "DateOfAccident", ErrorMessage = "DateOfAccident must be provided" });
            }

            if (errors.Count > 0) 
            { 
                return new CommonOutput { Result = RESULT.FAILURE, Output = errors };
            }

            // Delegate to the service layer; the service handles deeper business rules and persistence.
            CommonOutput result = await _claimDetailService.AddNewClaim(claimDetail);
            return result;
        }
        catch (MaximumClaimLimitReachedException ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return 
                new CommonOutput
                {
                    Result=RESULT.FAILURE,
                    Output= new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="PolicyNo",
                            ErrorMessage=ex.Message
                        }
                    }
                };
        }
        catch (Exception ex)
        {
            //Log exception and rethrow so higher layers (controllers/gRPC) can produce proper HTTP/gRPC responses.
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    }

    // GetClaimByClaimId:
    // - Calls into ClaimDetailService to fetch a claim DTO and wraps it into CommonOutput.
    // - Returns a success CommonOutput when found, otherwise failure.
    public async Task<CommonOutput> GetClaimByClaimId(string ClaimId)
    {

        try
        {
            ClaimListOpenDTO? output = await _claimDetailService.GetClaimByClaimId(ClaimId);
            if (output != null)
            {
                return new CommonOutput{
                    Output=output,
                    Result=RESULT.SUCCESS,
                };
            }
            return new CommonOutput{
                Result=RESULT.FAILURE
            };

        }
        catch(Exception ex)
        {
            //_logger.Error(LogMessage(   ex.Message));
            throw;
        }
    }

    // GetClaimStatusReports:
    // - Validates month/year range; throws an InvalidMonthOrYearException for invalid input.
    // - Delegates to ClaimDetailService.ClaimStatusReportsBasedOnMonthAndYear and maps result to CommonOutput.
    public async Task<CommonOutput> GetClaimStatusReports(int month, int year)
    {
        try
        {
            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }
            var reports = await _claimDetailService.ClaimStatusReportsBasedOnMonthAndYear(month, year);
            return new CommonOutput{
                Output=reports,
                Result=RESULT.SUCCESS
            };
        }
        catch(InvalidMonthOrYearException ex){
            return new CommonOutput{
                Output=ex.Message,
                Result=RESULT.FAILURE
            };
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    }

    // GetPaymentStatusReports:
    // - Validates month/year and forwards to ClaimDetailService.PaymentStatusBasedOnMonthAndYear.
    // - Returns CommonOutput with ClaimPaymentReportDTO on success.
    public async Task<CommonOutput> GetPaymentStatusReports(int month, int year)
    {
        try
        {
            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }
            var payments = await _claimDetailService.PaymentStatusBasedOnMonthAndYear(month, year);
            return new CommonOutput{
                Result=RESULT.SUCCESS,
                Output=payments
            };
        }
        catch (InvalidMonthOrYearException ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return new CommonOutput{
                Result = RESULT.FAILURE,
                Output = ex.Message
            };
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    }

    // UpdateClaimAmountApprovedBySurveyor:
    // - Thin facade that delegates to ClaimDetailService.UpdateClaimAmtApprovedBySurveyor.
    // - Propagates the service CommonOutput back to callers (controllers/gRPC).
    public async Task<CommonOutput> UpdateClaimAmountApprovedBySurveyor(string claimID, int claimant){
        try
        {
            CommonOutput result = await _claimDetailService.UpdateClaimAmtApprovedBySurveyor(claimID, claimant);
            return result;
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    }

}
