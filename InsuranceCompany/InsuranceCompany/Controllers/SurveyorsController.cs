using InsuranceCompany.BLL;
using Microsoft.AspNetCore.Mvc;

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

    //GET: /api/<SurveyorsController>/{EstimatedLoss}
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

    private string LogMessage(string message)
    {
        return "Ran with this problem " + message + " in SurveyorsController";
    }
}

