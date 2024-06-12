using Microsoft.AspNetCore.Mvc;
using SharedModules;
using Surveyor.BLL;
using Surveyor.DAL;

namespace Surveyor;

[Route("api/[controller]")]
[ApiController]
public class SurveyorsController:ControllerBase
{

    private readonly ISurveyorService _surveyorService;

    public SurveyorsController(ISurveyorService surveyorService){
        _surveyorService = surveyorService;
    }

    [HttpGet("{claimId}")]
    public async Task<IActionResult> GetSurveyReport(string claimId){
        try{
            ReportDTO? report=await _surveyorService.GetSurveyReport(claimId);
            if(report==null){
                return NotFound();
            }
            return Ok(report);
        }
        catch(Exception ex){
            return StatusCode(500,"Internal Surver Error");
        }

    }

    [HttpPost("new")]
    public async Task<IActionResult> AddSurveyReport(SurveyReportDTO surveyReport){
        
        try{
            CommonOutput output=await _surveyorService.AddNewSurveyReport(surveyReport);
            if(output.Result==RESULT.FAILURE){
                return BadRequest(output);
            }
            return Ok(output);
        }
        catch(Exception ex){
            return StatusCode(500,"Internal Surver Error");
        }
    }

    [HttpPatch("{claimId}")]
    public async Task<IActionResult> UpdateSurveyReport(string claimId,UpdateReportDTO updateReportDTO)
    {   
        try{
            CommonOutput output=await _surveyorService.UpdateSurveyReport(claimId,updateReportDTO);
            if(output.Result==RESULT.FAILURE){
                return BadRequest(output);
            }
            return Ok(output);
        }
        catch(Exception ex){
            return StatusCode(500, "Internal Server Error");
        }
    }


}
