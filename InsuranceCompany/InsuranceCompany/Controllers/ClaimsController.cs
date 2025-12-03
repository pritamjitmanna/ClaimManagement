// Summary:
// ASP.NET Core API controller exposing HTTP endpoints for claims-related operations.
// - Uses IClaimDetailService for business operations and ISharedLogic for lightweight validations/orchestration.
// - Action methods return appropriate HTTP status codes (200/204/400/404/500) depending on the service output.
// - Methods are asynchronous and rely on await for non-blocking DB/service calls.

using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace InsuranceCompany;

[Route("api")]
[ApiController]
public class ClaimsController : ControllerBase
{
    #pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
    private readonly IClaimDetailService _claimDetailService;
    private readonly ISharedLogic _sharedLogic;
    //private readonly ILog _logger;
    const string INTERNAL_SERVER_ERROR = "There's an unexpected Internal error. Sorry for the inconvenience caused. Please try again after some time";

    public ClaimsController(IClaimDetailService claimDetailService, ISharedLogic sharedLogic)
    {
        _claimDetailService = claimDetailService;
        _sharedLogic = sharedLogic;
        //_logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    // GetAllOpenClaims:
    // - Returns 200 with list of open claims, 204 when no content, and 500 for unexpected errors.
    // - Uses ClaimDetailService.ListAllOpenClaims to obtain DTOs.
    // GET: api/<ClaimsController>
    [HttpGet("[controller]")]
    [ProducesResponseType(typeof(IEnumerable<ClaimListOpenDTO>),StatusCodes.Status200OK,"application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClaimListOpenDTO>),StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string),StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOpenClaims()
    {

        try
        {
            IEnumerable<ClaimListOpenDTO> claims = await _claimDetailService.ListAllOpenClaims();
            if (claims.Count() == 0)
            {
                return StatusCode(204, claims);
            }
            return Ok(claims);

        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    // GetAllClosedClaims:
    // - Same pattern as open claims but uses ListAllClosedClaims.
    // GET: api/<ClaimsController>
    [HttpGet("[controller]/closed")]
    [ProducesResponseType(typeof(IEnumerable<ClaimListOpenDTO>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClaimListOpenDTO>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllClosedClaims()
    {
        try
        {
            IEnumerable<ClaimListOpenDTO> claims = await _claimDetailService.ListAllClosedClaims();
            if (claims.Count() == 0)
            {
                return StatusCode(204, claims);
            }
            return Ok(claims);

        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    // GetClaimByClaimId:
    // - Uses SharedLogic for retrieval and returns 200 when found or 404 when not found.
    [HttpGet("[controller]/{ClaimId}")]
    [ProducesResponseType(typeof(ClaimListOpenDTO), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetClaimByClaimId(string ClaimId)
    {
        try
        {

            CommonOutput output=await _sharedLogic.GetClaimByClaimId(ClaimId);
            if(output.Result==RESULT.SUCCESS){
                return Ok(output.Output);
            }
            return NotFound();

        }
        catch(Exception ex)
        {
            //_logger.Error(LogMessage(   ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }


    // GetClaimStatusReports:
    // - Endpoint validates via SharedLogic; returns 200 with report or 400 with validation message.
    // GET: api/claimStatus/report/{month}/{year}
    [HttpGet("claimStatus/report/{month}/{year}")]
    [ProducesResponseType(typeof(ClaimStatusReportDTO), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetClaimStatusReports(int month, int year)
    {
        try
        {

            CommonOutput output= await _sharedLogic.GetClaimStatusReports(month,year);
            if(output.Result==RESULT.SUCCESS){
                return Ok(output.Output);
            }
            return BadRequest(output.Output);
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    // GetPaymentStatusReports:
    // - Similar to claim status reports; returns ClaimPaymentReportDTO on success.
    //GET: api/paymentStatus/report/{month}/{year}
    [HttpGet("paymentStatus/report/{month}/{year}")]
    [ProducesResponseType(typeof(ClaimPaymentReportDTO), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status400BadRequest,"application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentStatusReports(int month, int year)
    {
        try
        {

            CommonOutput output=await _sharedLogic.GetPaymentStatusReports(month,year);

            if(output.Result==RESULT.SUCCESS){
                return Ok(output.Output);
            }
            return BadRequest(output.Output);
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    // AddNewClaim:
    // - Consumes JSON body and delegates validation+creation to SharedLogic.AddClaimSharedLogic.
    // - Returns 200 on success or 400/500 accordingly.
    //POST: api/<ClaimsController>/new
    [HttpPost("[controller]/new")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CommonOutput),StatusCodes.Status200OK,"application/json")]
    [ProducesResponseType(typeof(CommonOutput),StatusCodes.Status400BadRequest,"application/json")]
    [ProducesResponseType(typeof(string),StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddNewClaim([FromBody] ClaimDetailRequestDTO claimDetail)
    {
        try{
            CommonOutput result=await _sharedLogic.AddClaimSharedLogic(claimDetail);
            if(result.Result==RESULT.SUCCESS){
                return Ok(result);
            }
            return BadRequest(result);  
        }
        catch(Exception ex){
            return StatusCode(500, new CommonOutput { Result=RESULT.FAILURE,Output=INTERNAL_SERVER_ERROR});
        }


    }


    // UpdateClaim:
    // - Delegates to ClaimDetailService.UpdateClaim; returns 200 on success, 400 on validation failure.
    // PUT api/<ClaimsController>/{claimID}/update
    [HttpPut("[controller]/{claimID}/update")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CommonOutput),StatusCodes.Status200OK,"application/json")]
    [ProducesResponseType(typeof(CommonOutput),StatusCodes.Status400BadRequest,"application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateClaim(string claimID, [FromBody] UpdateClaimDTO updateDTO)
    {
        try
        {
            var result = await _claimDetailService.UpdateClaim(claimID, updateDTO);
            if (result.Result == RESULT.SUCCESS)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, ex.Message);
        }
    }

    // UpdateClaimAmountApprovedBySurveyor:
    // - PATCH endpoint to set the amount approved by surveyor via SharedLogic facade.
    //PATCH api/<ClaimsController>/{claimID}/{claimant}/update
    [HttpPatch("[controller]/{claimID}/{claimant}/update")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateClaimAmountApprovedBySurveyor(string claimID, int claimant)
    {
        try
        {
            CommonOutput result = await _sharedLogic.UpdateClaimAmountApprovedBySurveyor(claimID,claimant);
            if (result.Result == RESULT.SUCCESS)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    // ReleaseSurveyorFees:
    // - Calls service to compute surveyor fees by estimated loss and persist them to the claim.
    //PATCH: api/surveyorfees/{claimId}
    [HttpPatch("surveyorfees/{claimId}")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReleaseSurveyorFees(string claimId)
    {
        try
        {
            CommonOutput result = await _claimDetailService.UpdateClaimSurveyorFees(claimId);
            if (result.Result == RESULT.SUCCESS)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, INTERNAL_SERVER_ERROR);
        }
    }

    private string LogMessage(string message)
    {
        return "Ran with this problem " + message + " in ClaimsController";
    }
}

