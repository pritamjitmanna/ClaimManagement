﻿using InsuranceCompany.BLL;
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
            //Log exception
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    }

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
