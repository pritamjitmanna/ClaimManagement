using Insured.BLL;
using Microsoft.AspNetCore.Mvc;
using SharedModules;

namespace Insured;

[Route("api/claims")]
[ApiController]
public class InsuredController:ControllerBase
{

    private readonly IInsuredService _insuredService;
    public InsuredController(IInsuredService insuredService){
        _insuredService = insuredService;
    }

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
            return StatusCode(500,"Internal Server Error");
        }
    }

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
            return StatusCode(500,"Internal Server Error");
        }
    }

}
