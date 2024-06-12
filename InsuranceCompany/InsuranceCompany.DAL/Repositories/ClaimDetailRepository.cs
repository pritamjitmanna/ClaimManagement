using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceCompany.DAL;
using SharedModules;

public class ClaimDetailRepository : IClaimDetail
{

    private readonly InsuranceCompanyDBContext _dbContext;
    //private readonly ILog _logger;

    public ClaimDetailRepository(InsuranceCompanyDBContext dbContext)
    {
        _dbContext = dbContext;
    }



    
    public async Task<CommonOutput> AddNewClaim(ClaimDetail claimDetail)
    {
        CommonOutput result;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            bool IsValid = ValidationFunctions.ValidateModel(claimDetail, ref validationResults);

            if (!IsValid)
            {

                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = validationResults
                };
            }
            else
            {
                await _dbContext.ClaimDetails.AddAsync(claimDetail);
                await _dbContext.SaveChangesAsync();
                result = new CommonOutput
                {
                    Result = RESULT.SUCCESS,
                    Output = claimDetail.ClaimId
                };
            }


        }
        catch (Exception ex)
        {
            //Log the Exception\
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        return result;
    }

    public async Task<ICollection<ClaimDetail>> GetAllOpenClaims()
    {
        ICollection<ClaimDetail> claims = new List<ClaimDetail>();
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            claims=await _dbContext.ClaimDetails.AsNoTracking().Where(c=>c.ClaimStatus==ClaimStatus.Open).ToListAsync();

            //claims = await (from x in _dbContext.ClaimDetails.AsNoTracking() where x.ClaimStatus == ClaimStatus.Open select x).ToListAsync();
        }
        catch (Exception ex)
        {
            //Log the exception
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return claims;
    }

    public async Task<int> PaymentStatusOnMonthAndYear(int month, int year)
    {

        int totalPayment = 0;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            totalPayment = (int)await _dbContext.ClaimDetails.AsNoTracking().Where(cd=>cd.DateOfAccident.Month==month).Where(cd=>cd.DateOfAccident.Year==year).Where(cd=>cd.InsuranceCompanyApproval==true).Select(cd=>cd.AmtApprovedBySurveyor).SumAsync();

            //totalPayment = (int)await (from x in _dbContext.ClaimDetails.AsNoTracking() where x.DateOfAccident.Month == month && x.DateOfAccident.Year == year && x.InsuranceCompanyApproval == true select x.AmtApprovedBySurveyor).SumAsync();
        }
        catch (Exception ex)
        {
            //Log Error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used

        return totalPayment;
    }




    public async Task<int> GetClaimsCountForStageTypeBasedOnMonthAndYear(Stages stage, int month, int year)
    {
        int count = 0;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            if (stage == Stages.NewClaims)
            {
                count = await _dbContext.ClaimDetails.AsNoTracking().Where(cd => cd.DateOfAccident.Month == month).Where(cd => cd.DateOfAccident.Year == year).Where(cd => cd.AmtApprovedBySurveyor == null || cd.AmtApprovedBySurveyor == 0).Where(cd => cd.ClaimStatus == ClaimStatus.Open).Where(cd => cd.InsuranceCompanyApproval == false).CountAsync();
            }
            else if (stage == Stages.PendingClaims)
            {
                count = await _dbContext.ClaimDetails.AsNoTracking().Where(cd => cd.DateOfAccident.Month == month).Where(cd => cd.DateOfAccident.Year == year).Where(cd => cd.AmtApprovedBySurveyor > 0).Where(cd => cd.ClaimStatus == ClaimStatus.Open).Where(cd => cd.InsuranceCompanyApproval == false).Where(cd => cd.WithdrawClaim == WITHDRAWSTATUS.NOSTATUS || cd.WithdrawClaim==WITHDRAWSTATUS.ACCEPTED).CountAsync();
            }
            else if (stage == Stages.FinalizedClaims)
            {
                count = await _dbContext.ClaimDetails.AsNoTracking().Where(cd => cd.DateOfAccident.Month == month).Where(cd => cd.DateOfAccident.Year == year).Where(cd => cd.ClaimStatus == ClaimStatus.Closed).Where(cd => cd.InsuranceCompanyApproval == true).Where(cd => cd.WithdrawClaim == WITHDRAWSTATUS.NOSTATUS||cd.WithdrawClaim==WITHDRAWSTATUS.ACCEPTED).CountAsync();
            }
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return count;
    }


    public async Task<CommonOutput> UpdateClaim(ClaimDetail claimDetail)
    {

        CommonOutput result;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            bool IsValid = ValidationFunctions.ValidateModel(claimDetail, ref validationResults);

            if (!IsValid)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = validationResults
                };
            }
            else
            {
                _dbContext.Update(claimDetail);
                await _dbContext.SaveChangesAsync();
                result = new CommonOutput
                {
                    Result = RESULT.SUCCESS
                };
            }

        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        return result;
    }




    //--Other functions--

    public async Task<ClaimDetail?> GetClaimByPolicyNo(string policyNo)
    {

        ClaimDetail? claim;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            claim = await _dbContext.ClaimDetails.AsNoTracking().Where(cd => cd.PolicyNo == policyNo).FirstOrDefaultAsync();
            //claim = await (from x in _dbContext.ClaimDetails.AsNoTracking() where x.PolicyNo == policyNo select x).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return claim;
    }

    public async Task<ClaimDetail?> GetClaimByClaimId(string claimId)
    {

        ClaimDetail? claim;
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            claim = await _dbContext.ClaimDetails.AsNoTracking().Where(cd => cd.ClaimId == claimId).FirstOrDefaultAsync();
            //claim = await (from x in _dbContext.ClaimDetails.AsNoTracking() where x.ClaimId == claimId select x).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            //Log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return claim;
    }

    public async Task<ICollection<ClaimDetail>> GetAllCloseClaims()
    {
        ICollection<ClaimDetail> claims = new List<ClaimDetail>();
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            claims = await _dbContext.ClaimDetails.AsNoTracking().Where(c => c.ClaimStatus == ClaimStatus.Closed).ToListAsync();
        }
        catch (Exception ex)
        {
            //Log the exception
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        return claims;
    }


    private string LogMessage(string message)
    {
        return "Ran with this problem " + message + " in ClaimDetailRepository";
    }
}
