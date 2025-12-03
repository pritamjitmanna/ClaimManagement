// Summary:
// ClaimDetailRepository manages ClaimDetail entities: adding, updating, and querying claims.
// It demonstrates model validation prior to persistence, and various read queries using EF Core's LINQ providers.
// Important EF Core functions used:
// - AddAsync/SaveChangesAsync: to persist new entities.
// - AsNoTracking: for read-only queries to avoid change tracking overhead.
// - Where: filter records; can be chained for multiple conditions.
// - SumAsync/CountAsync: aggregate functions executed on the database side.
// - Update: marks an entity as modified and SaveChangesAsync persists the change.

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

    // Adds a new ClaimDetail after validating the model.
    // ValidationFunctions.ValidateModel: custom validation routine that populates ValidationResult collection.
    // If valid: AddAsync (asynchronous add to change tracker) followed by SaveChangesAsync to persist to DB.
    // Returns CommonOutput containing success/failure and either ClaimId or validation errors.
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

    // Returns all claims with ClaimStatus.Open using AsNoTracking and ToListAsync.
    // AsNoTracking improves query performance since returned entities are not tracked for changes.
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

    // Computes total payments for claims that occurred in a specific month/year and have company approval.
    // Uses SumAsync to compute sum at DB level; Select projects AmtApprovedBySurveyor values.
    // Note: casting to int assumes the resulting sum is non-nullable; be careful in real-world code for possible null.
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



    // Returns counts for different stages (NewClaims, PendingClaims, FinalizedClaims) for a given month/year.
    // Uses CountAsync which is translated to a SQL COUNT(*) with provided filters.
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


    // Update a claim after validation. Update marks the entity as Modified in the context.
    // SaveChangesAsync then persists the change to the database.
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

    // Retrieves a single ClaimDetail by PolicyNo. Uses FirstOrDefaultAsync to return null if not found.
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

    // Retrieves a single ClaimDetail by ClaimId.
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

    // Gets all closed claims. ToListAsync materializes results into a collection.
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
