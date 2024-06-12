using System.ComponentModel.DataAnnotations;
using AutoMapper;
using gRPCClaimsService.Protos;
using SharedModules;
using Surveyor.DAL;

namespace Surveyor.BLL;

public class SurveyorService:ISurveyorService
{

    private readonly ISurveyor _surveyorRepository;
    private readonly ClaimsService.ClaimsServiceClient _claimsServiceClient;
    private readonly IMapper _mapper;

    public SurveyorService(ISurveyor surveyorRepository,ClaimsService.ClaimsServiceClient claimsServiceClient,IMapper mapper){
        _surveyorRepository = surveyorRepository;
        _claimsServiceClient = claimsServiceClient;
        _mapper = mapper;
    }

    public async Task<CommonOutput> AddNewSurveyReport(SurveyReportDTO surveyReport){
        ClaimDTOgRPC claim;
        PolicyDTOgRPC policy;
        CommonOutput output;
        try
        {
            CommonOutputgRPC result=await _claimsServiceClient.GetClaimByClaimIdAsync(new GetClaimByIdString{
                ClaimId=surveyReport.ClaimId
            });

            if(result.StatusCode==STATUSCODE.Ok){
                if(result.Output.TryUnpack(out ClaimDTOgRPC tempclaim)){

                    if(tempclaim.ClaimStatus==CLAIMSTATUS.Closed || tempclaim.WithdrawClaim==gRPCClaimsService.Protos.WITHDRAWSTATUS.Withdrawn){
                        return new CommonOutput{
                            Result=RESULT.FAILURE,
                            Output=new List<PropertyValidationResponse>{
                                new() {
                                    Property="ClaimId",
                                    ErrorMessage="The Claim is either closed or withdrawn."
                                }
                            }
                        };
                    }

                    claim = tempclaim;
                }
                else{
                    throw new Exception();
                }
            }
            else if(result.StatusCode==STATUSCODE.Notfound){
                return new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=new List<PropertyValidationResponse>{
                        new() {
                            Property="ClaimId",
                            ErrorMessage="No Claim exist with the given Id"
                        }
                    }
                };
            }
            else{
                throw new Exception();
            }

            result=await _claimsServiceClient.GetPolicyByPolicyNoAsync(new GetPolicyNoString{
                PolicyNo=surveyReport.PolicyNo
            });
            

            if(result.StatusCode==STATUSCODE.Ok){
                if(result.Output.TryUnpack(out PolicyDTOgRPC temppolicy)){
                    policy = temppolicy;
                }
                else{
                    throw new Exception();
                }
            }
            else if(result.StatusCode==STATUSCODE.Notfound){
                return new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=new List<PropertyValidationResponse>{
                        new() {
                            Property="PolicyNo",
                            ErrorMessage="No Policy exist with the given PolicyNo"
                        }
                    }
                };
            }
            else{
                throw new Exception();
            }

            SurveyReport report=_mapper.Map<SurveyReport>(surveyReport);

            int VehicleAge=GetVehicleAge(policy);
            int EstimatedLoss=claim.EstimatedLoss;
            int PolicyClause=0;
            if(VehicleAge<5)PolicyClause=500;
            else if(VehicleAge<10)PolicyClause=1500;
            else PolicyClause=5000;

            int TotalAmount=surveyReport.PartsCost+surveyReport.LabourCharges-PolicyClause;

            report.VehicleAge=VehicleAge;
            report.EstimatedLoss=EstimatedLoss;
            report.PolicyClause=PolicyClause;
            report.TotalAmount=TotalAmount;

            output=await _surveyorRepository.AddSurveyReport(report);

            GetErrorListInRequiredFormat(ref output);

            if(output.Result==RESULT.SUCCESS){
                    output=await UpdateClaimAmtInsuranceCompany(report.ClaimId,TotalAmount);
                    if(output.Result==RESULT.FAILURE){
                        List<PropertyValidationResponse> temp=(List<PropertyValidationResponse>)output.Output;
                        temp.Add(new PropertyValidationResponse{
                            Property="Claim",
                            ErrorMessage="There's some error while updating the Amount in InsuranceCompany, try again after sometime."
                        });
                        output.Output=temp;
                    }
                }
            
        }
        catch (Exception ex)
        {
            throw;
        }

        return output;

    }

    public async Task<ReportDTO?> GetSurveyReport(string claimId){
        try{
            SurveyReport? report=await _surveyorRepository.GetSurveyReport(claimId);
            if(report==null)return null;
            return _mapper.Map<ReportDTO>(report);
        }
        catch(Exception ex){
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<CommonOutput> UpdateSurveyReport(string claimId,UpdateReportDTO updateReportDTO){
        CommonOutput result;
        try{
            SurveyReport? surveyReport=await _surveyorRepository.GetSurveyReport(claimId);
            if(surveyReport==null){
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="No Report exist with the claimId"
                        }
                    }
                };
            }
            else{
                int EstimatedLoss=surveyReport.EstimatedLoss;
                int PolicyClause=surveyReport.PolicyClause;
                int PartsCost=surveyReport.PartsCost;
                int LabourCharges=surveyReport.LabourCharges;
                if(updateReportDTO.LabourCharges!=null){
                    LabourCharges=(int)updateReportDTO.LabourCharges;
                    surveyReport.LabourCharges=(int)updateReportDTO.LabourCharges;
                }
                if(updateReportDTO.PartsCost!=null){
                    PartsCost=(int)updateReportDTO.PartsCost;
                    surveyReport.PartsCost=(int)updateReportDTO.PartsCost;
                }

                int TotalAmount=surveyReport.PartsCost+surveyReport.LabourCharges-PolicyClause;

                surveyReport.TotalAmount=TotalAmount;
                surveyReport.LabourCharges=LabourCharges;
                surveyReport.PartsCost=PartsCost;
                
                if(updateReportDTO.AccidentDetails!=null)surveyReport.AccidentDetails=updateReportDTO.AccidentDetails;
                if(updateReportDTO.DepreciationCost!=null)surveyReport.DepreciationCost=(int)updateReportDTO.DepreciationCost;

                result=await _surveyorRepository.UpdateSurveyReport(surveyReport);

                GetErrorListInRequiredFormat(ref result);

                if(result.Result==RESULT.SUCCESS){
                    result=await UpdateClaimAmtInsuranceCompany(claimId,TotalAmount);
                    if(result.Result==RESULT.FAILURE){
                        List<PropertyValidationResponse> temp=(List<PropertyValidationResponse>)result.Output;
                        temp.Add(new PropertyValidationResponse{
                            Property="Claim",
                            ErrorMessage="There's some error while updating the Amount in InsuranceCompany, try again after sometime."
                        });
                        result.Output=temp;
                    }
                }
            }
        }   
        catch(Exception ex){
            throw;
        }
        return result;
    }


    public async Task<CommonOutput> UpdateClaimAmtInsuranceCompany(string claimId,int claimant){
        try{
            CommonOutputgRPC output=await _claimsServiceClient.UpdateClaimAmountApprovedBySurveyorAsync(new UpdateClaimAmountApprovedBySurveyorClaimIdClaimant{
                ClaimID=claimId,
                Claimant=claimant
            });

            if(output.StatusCode==STATUSCODE.Ok){
                return new CommonOutput{
                    Result=RESULT.SUCCESS
                };
            }
            else if(output.StatusCode==STATUSCODE.Badrequest){
                if(output.Output.TryUnpack(out ErrorsListgRPC errs)){
                    List<PropertyValidationResponse> errors=[];
                    foreach(var error in errs.Errors){
                        errors.Add(_mapper.Map<PropertyValidationResponse>(error));
                    }
                    return new CommonOutput{
                        Result=RESULT.FAILURE,
                        Output=errors
                    };
                }
                else{
                    throw new Exception();
                }
            }
            else{
                throw new Exception();
            }
        }
        catch(Exception ex){
            throw;
        }
    } 



    //Helper

    private int GetVehicleAge(PolicyDTOgRPC policy){
        DateTime curr=DateTime.Now;
        DateTime Ins=policy.DateOfInsurance.ToDateTime();
        int age=curr.Year-Ins.Year;
        if(Ins.Month>curr.Month)age--;
        else if(Ins.Month==curr.Month && Ins.Day>curr.Day)age--;

        return age;
    }

    private void GetErrorListInRequiredFormat(ref CommonOutput result)
    {
        if (result.Result == RESULT.FAILURE)
        {
            List<PropertyValidationResponse> validationErrors = new List<PropertyValidationResponse>();

            foreach (var err in (ICollection<ValidationResult>)result.Output)
            {
                validationErrors.Add(
                    new PropertyValidationResponse
                    {
                        Property = err.MemberNames.First(),
                        ErrorMessage = err.ErrorMessage
                    });
            }

            result.Output = validationErrors;
        }
    }

}
