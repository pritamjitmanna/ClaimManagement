using System.Security.AccessControl;
using Grpc.Core;
using Grpc.Net.Client;
using IRDA.BLL;
using IRDA.DAL;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace IRDA;

/// <summary>
/// API controller for IRDA reporting endpoints.
/// - Provides endpoints to fetch and pull claim-status and payment-status reports for a given month/year.
/// - Delegates business logic to BLL services and maps service outputs to HTTP responses:
///   - Returns 200 OK with data on success.
///   - Returns 400 Bad Request when input validation or business validation fails.
///   - Returns 500 Internal Server Error for unexpected exceptions.
/// </summary>
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


    /// <summary>
    /// GET api to fetch aggregated claim status report for the given month and year.
    /// - Validates month/year range; throws InvalidMonthOrYearException for invalid inputs.
    /// - Calls BLL to retrieve data and returns it as 200 OK.
    /// - Converts InvalidMonthOrYearException to 400 Bad Request with message.
    /// - Converts unexpected exceptions to 500 with a user-friendly message.
    /// </summary>
    [HttpGet("claimStatus/report/{month}/{year}")]
    public async Task<IActionResult> GetClaimStatus(int month,int year){
        try{
            // Validate month/year manually to avoid meaningless requests
            if (month < 1 || year < 0 || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year || month > 12)
            {
                throw new InvalidMonthOrYearException("Invalid month or Year");
            }
            // Delegate to service to retrieve pending status report data
            IEnumerable<ClaimStatusReportDTO> output=await _pendingStatusReports.GetPendingStatusReports(month,year);
            return Ok(output);
        }
        catch(InvalidMonthOrYearException ex){
            // Return validation error
            return BadRequest(ex.Message);
        }
        catch(Exception ex){
            // Generic internal error mapping; log the exception in production.
            return StatusCode(500,INTERNAL_SERVER_ERROR);
        }
    }


    /// <summary>
    /// GET api to fetch payment status for the given month and year.
    /// - Validates inputs, delegates to service, and maps errors similarly to GetClaimStatus.
    /// </summary>
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

    /// <summary>
    /// Pulls claim status reports from the remote claims service and stores/updates them locally.
    /// - Delegates to PendingStatusReportsService.AddPendingStatusReports which calls gRPC, maps and persists data.
    /// - Returns BadRequest when business validation fails (e.g., mapped validation errors).
    /// </summary>
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
            // Here the exception message is returned directly; consider returning a fixed message in production.
            return StatusCode(500,ex.Message);
        }
    }


    /// <summary>
    /// Pulls payment status from the remote claims service and stores/updates locally.
    /// - Delegates to PaymentOfClaimsService.AddPaymentOfClaimsStatus.
    /// - Maps failure result to 400 Bad Request; otherwise returns 200 OK.
    /// </summary>
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
