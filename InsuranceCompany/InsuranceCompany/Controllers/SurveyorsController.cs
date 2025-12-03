// Summary:
// Exposes an API endpoint to list surveyors based on estimated loss. Delegates to ISurveyorService
// and returns 200/204/500 depending on result. The controller action is asynchronous and uses AutoMapper results provided by the service.

using InsuranceCompany.BLL;
using InsuranceCompany.BLL.RequestDTO;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace InsuranceCompany;

[Route("api/[controller]")]
[ApiController]
public class SurveyorsController : ControllerBase
{
    private readonly ISurveyorService _surveyorService;
    //private readonly ILog _logger;
    public SurveyorsController(ISurveyorService surveyorService)
    {
        _surveyorService = surveyorService;
        //_logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    // GetAllSurveyorsOnEstimatedLoss:
    // - Accepts EstimatedLoss as route parameter, calls service to fetch DTO list.
    // - Returns 200 with list, 204 when empty, or 500 on unexpected errors.
    [HttpGet("{EstimatedLoss}")]
    [ProducesResponseType(typeof(IEnumerable<SurveyorDTO>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(IEnumerable<SurveyorDTO>), StatusCodes.Status204NoContent, "application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllSurveyorsOnEstimatedLoss(int EstimatedLoss)
    {

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        IEnumerable<SurveyorDTO> surveyors = new List<SurveyorDTO>();
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        try
        {
            surveyors = await _surveyorService.GetSurveyorListOnEstimatedLoss(EstimatedLoss);
            if (surveyors.Count() == 0)
            {
                return StatusCode(StatusCodes.Status204NoContent, surveyors);
            }
            return Ok(surveyors);
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("addsurveyor")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(CommonOutput), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddSurveyorDetails(SurveyorEntryDTO surveyorEntryDTO)
    {
        try
        {
            CommonOutput result=await _surveyorService.AddSurveyorDetails(surveyorEntryDTO);
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

    [HttpDelete("deletesurveyor/{surveyorId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSurveyorDetails(int surveyorId)
    {
        try
        {
            bool result = await _surveyorService.DeleteSurveyorDetails(surveyorId);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            return StatusCode(500, ex.Message);
        }
    }

    private string LogMessage(string message)
    {
        return "Ran with this problem " + message + " in SurveyorsController";
    }
}

