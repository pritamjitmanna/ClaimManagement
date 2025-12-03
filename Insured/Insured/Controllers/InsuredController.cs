using Insured.BLL;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace Insured;

/// <summary>
/// API controller exposing endpoints for insured users to submit claims and accept/reject claim outcomes.
/// - Route prefix: api/claims
/// - Contains:
///   - POST api/claims/addclaim : submit a new claim payload (ClaimDetailRequestDTO).
///   - PATCH api/claims/{claimId} : accept or reject an existing claim by id.
/// 
/// Controller responsibilities:
/// - Call the InsuredService for business logic and remote gRPC calls.
/// - Translate service CommonOutput into appropriate HTTP responses:
///   - RESULT.SUCCESS -> 200 OK (optionally with payload)
///   - RESULT.FAILURE -> 400 Bad Request (with validation errors as payload)
///   - Exceptions -> 500 Internal Server Error (generic message returned)
/// </summary>
[Route("api/claims")]
[ApiController]
public class InsuredController:ControllerBase
{

    private readonly IInsuredService _insuredService;
    public InsuredController(IInsuredService insuredService){
        _insuredService = insuredService;
    }

    /// <summary>
    /// Accepts a ClaimDetailRequestDTO in the request body and forwards it to the service to create a new claim.
    /// - On success returns 200 OK with the CommonOutput containing the claim id.
    /// - On validation failure returns 400 Bad Request with validation details.
    /// - On unexpected exceptions returns 500 Internal Server Error.
    /// </summary>
    [HttpPost("addclaim")]
    public async Task<IActionResult> AddNewClaim([FromBody]ClaimDetailRequestDTO claim){
        try{
            CommonOutput output = await _insuredService.AddNewClaim(claim);
            if(output.Result==RESULT.SUCCESS){
                return Ok(output);
            }
            return BadRequest(output);
        }
        catch(Exception ex){
            // For simplicity a generic 500 is returned. In production expose minimal info and log the exception.
            return StatusCode(500,"Internal Server Error");
        }
    }

    /// <summary>
    /// Accepts or rejects a claim identified by claimId.
    /// - The AcceptRejectDTO contains a boolean flag indicating acceptance or rejection.
    /// - Returns 200 OK on success, 400 Bad Request with errors on validation failure, or 500 on unexpected error.
    /// </summary>
    [HttpPatch("{claimId}")]
    public async Task<IActionResult> AcceptOrRejectClaim(string claimId,AcceptRejectDTO acceptReject)
    {
        try{
            CommonOutput output= await _insuredService.AcceptOrRejectClaim(claimId,acceptReject);
            if(output.Result==RESULT.SUCCESS){
                return Ok();
            }
            return BadRequest(output.Output);
        }
        catch(Exception ex){
            // Generic error mapping; consider returning structured error objects and logging the detailed exception.
            return StatusCode(500,"Internal Server Error");
        }
    }

}
