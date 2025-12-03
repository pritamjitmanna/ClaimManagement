using Microsoft.AspNetCore.Mvc;
using SharedModules;
using Surveyor.BLL;
using Surveyor.DAL;

namespace Surveyor;

/// <summary>
/// Controller exposing surveyor-facing endpoints:
/// - GET api/surveyors/{claimId}     : retrieve survey report by claim id.
/// - POST api/surveyors/new          : add a new survey report.
/// - PATCH api/surveyors/{claimId}   : update an existing survey report.
/// 
/// Responsibilities:
/// - Delegate business logic to ISurveyorService.
/// - Map service results to HTTP responses (200 OK, 400 Bad Request, 404 NotFound, 500 Internal Server Error).
/// </summary>
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
            // Calls service to fetch report DTO; returns 404 when not found to indicate resource absence.
            ReportDTO? report=await _surveyorService.GetSurveyReport(claimId);
            if(report==null){
                return NotFound();
            }
            return Ok(report);
        }
        catch(Exception ex){
            // Generic 500 response for unexpected errors; in production log details.
            return StatusCode(500,"Internal Surver Error");
        }

    }

    [HttpPost("new")]
    public async Task<IActionResult> AddSurveyReport(SurveyReportDTO surveyReport){
        
        try{
            // Add a new survey report using service; service returns CommonOutput with validation errors when applicable.
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
            // Update a report: returns 400 when validation fails, 200 on success.
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
