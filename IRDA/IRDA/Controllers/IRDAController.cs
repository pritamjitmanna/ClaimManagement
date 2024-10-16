using System.Security.AccessControl;
using Grpc.Core;
using Grpc.Net.Client;
using IRDA.BLL;
using IRDA.DAL;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace IRDA;

[Route("[controller]")]
[ApiController]
public class IRDAController:ControllerBase
{
    const string INTERNAL_SERVER_ERROR = "There's an unexpected Internal error. Sorry for the inconvenience caused. Please try again after some time";

    private readonly IPaymentOfClaimsService _paymentOfClaimsService;
    private readonly IPendingStatusReportsService _pendingStatusReports;

    public IRDAController(IPaymentOfClaimsService paymentOfClaimsService,IPendingStatusReportsService pendingStatusReports){
        _paymentOfClaimsService=paymentOfClaimsService;
        _pendingStatusReports=pendingStatusReports;
    }


    [HttpGet("claimStatus/report/{month}/{year}")]
    public async Task<IActionResult> GetClaimStatus(int month,int year){
        try{
            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }
            IEnumerable<ClaimStatusReportDTO> output=await _pendingStatusReports.GetPendingStatusReports(month,year);
            return Ok(output);
        }
        catch(InvalidMonthOrYearException ex){
            return BadRequest(ex.Message);
        }
        catch(Exception ex){
            return StatusCode(500,INTERNAL_SERVER_ERROR);
        }
    }


    [HttpGet("paymentStatus/report/{month}/{year}")]
    public async Task<IActionResult> GetPaymentStatus(int month,int year){
        try{
            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }
            ClaimPaymentReportDTO output=await _paymentOfClaimsService.GetPaymentStatus(month,year);
            return Ok(output);
        }
        catch(InvalidMonthOrYearException ex){
            return BadRequest(ex.Message);
        }
        catch(Exception ex){
            return StatusCode(500,INTERNAL_SERVER_ERROR);
        }
    }

    [HttpGet("claimStatus/pull/{month}/{year}")]
    public async Task<IActionResult> GetAndAddClaimStatus(int month,int year){

        try{
            CommonOutput output=await _pendingStatusReports.AddPendingStatusReports(month,year);
            if(output.Result==RESULT.FAILURE){
                return BadRequest(output);
            }
            return Ok(output);
        }
        catch(Exception ex){
            return StatusCode(500,ex.Message);
        }
    }


    [HttpGet("paymentStatus/pull/{month}/{year}")]
    public async Task<IActionResult> GetAndAddPaymentStatus(int month,int year){
        try{
            CommonOutput output=await _paymentOfClaimsService.AddPaymentOfClaimsStatus(month,year);
            if(output.Result==RESULT.FAILURE){
                return BadRequest(output);
            }
            return Ok(output);
        }
        catch(Exception ex){
            return StatusCode(500,INTERNAL_SERVER_ERROR);
        }
    }
}
