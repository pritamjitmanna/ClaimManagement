namespace InsuranceCompany.DAL;
using SharedModules;

///<summary>
///This is the Interface for using the ClaimDetail Repository which deals with all the CRUD operation in the ClaimDetails table
///</summary>
public interface IClaimDetail
{

    Task<ICollection<ClaimDetail>> GetAllCloseClaims();

    /// <summary>
    /// This function returns the list of all open claims in the claim detail table.
    /// </summary>
    /// <returns>IEnumerable<ClaimDetail></returns>
    Task<ICollection<ClaimDetail>> GetAllOpenClaims();

    /// <summary>
    /// This function adds a new claim to the claimDetails table. It first checks for the Validation errors using the ValidationContext, then adds the details.
    /// It returns the CommonOutput object with FAILURE/SUCCESS with the Output for validation errors or claimId.
    /// </summary>
    /// <param name="claimDetail"></param>
    /// <returns>CommonOutput</returns>
    Task<CommonOutput> AddNewClaim(ClaimDetail claimDetail);

    /// <summary>
    /// This function updates a claim in the claimDetails table. The claimDetail should contain the primary key value and the changed values in the object.
    /// The CommonOutput return FAILURE/SUCCESS with validation errors for FAILURE or nothing for SUCCESS.
    /// </summary>
    /// <param name="claimDetail"></param>
    /// <returns>CommonOutput</returns>
    Task<CommonOutput> UpdateClaim(ClaimDetail claimDetail);


    /// <summary>
    /// This function returns the total sum that is paid by Insurance Company in the given month and year.
    /// </summary>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns>int</returns>
    Task<int> PaymentStatusOnMonthAndYear(int month, int year);

    /// <summary>
    /// This function returns the total count of claims for the given stage(New/Pending/Finalized) of claims in the particular month and year.
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns>int</returns>
    Task<int> GetClaimsCountForStageTypeBasedOnMonthAndYear(Stages stage, int month, int year);

    /// <summary>
    /// This function returns the ClaimDetail object for a particular PolicyNo. It can be NULL also.
    /// </summary>
    /// <param name="policyNo"></param>
    /// <returns>ClaimDetail?</returns>
    Task<ClaimDetail?> GetClaimByPolicyNo(string policyNo);

    /// <summary>
    /// This function returns the ClaimDetail object for a particular ClaimId
    /// </summary>
    /// <param name="claimId"></param>
    /// <returns>ClaimDetail?</returns>
    Task<ClaimDetail?> GetClaimByClaimId(string claimId);


}
