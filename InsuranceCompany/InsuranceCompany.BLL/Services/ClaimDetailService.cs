// Summary:
// Service layer orchestrating claim-related business logic. Coordinates between repositories/services:
// - Validates business rules (policy existence, date checks, max claims per year).
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class ClaimDetailService : IClaimDetailService
{
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used

#pragma warning disable CS8604 // Possible null reference argument.

#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    private readonly IClaimDetail _claimDetailRepository;
    private readonly IPolicyService _policyService;
    private readonly IFeeService _feeService;
    private readonly ISurveyorService _surveyorService;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;

    public ClaimDetailService(IClaimDetail claimDetailRepository, IPolicyService policyService, IFeeService feeService, ISurveyorService surveyorService, IMapper mapper)
    {
        _claimDetailRepository = claimDetailRepository;
        _policyService = policyService;
        _feeService = feeService;
        _surveyorService = surveyorService;
        _mapper = mapper;
        //_logger = logger;
    }


    // ListAllOpenClaims:
    // - Calls repository to get open claims (DAL returns collection).
    // - Uses AutoMapper to map each ClaimDetail to ClaimListOpenDTO.
    // - 'await' releases thread while DB I/O completes.
    public async Task<IEnumerable<ClaimListOpenDTO>> ListAllOpenClaims()
    {

        ICollection<ClaimDetail> openClaims;
        List<ClaimListOpenDTO> result = new List<ClaimListOpenDTO>();
        try
        {
            openClaims = await _claimDetailRepository.GetAllOpenClaims();


            foreach (ClaimDetail claimDetail in openClaims)
            {
                ClaimListOpenDTO value = _mapper.Map<ClaimListOpenDTO>(claimDetail);
                result.Add(value);
            }
        }
        catch (Exception ex)
        {
            //log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }

    // ListAllClosedClaims: same pattern as ListAllOpenClaims but using GetAllCloseClaims.
    public async Task<IEnumerable<ClaimListOpenDTO>> ListAllClosedClaims()
    {

        ICollection<ClaimDetail> closeClaims;
        List<ClaimListOpenDTO> result = new List<ClaimListOpenDTO>();
        try
        {
            closeClaims = await _claimDetailRepository.GetAllCloseClaims();


            foreach (ClaimDetail claimDetail in closeClaims)
            {
                ClaimListOpenDTO value = _mapper.Map<ClaimListOpenDTO>(claimDetail);
                result.Add(value);
            }
        }
        catch (Exception ex)
        {
            //log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }

    // ClaimStatusReportsBasedOnMonthAndYear:
    // - Uses Enum.GetValues(typeof(Stages)) to iterate enum values (built-in reflection-based method).
    // - Calls repository GetClaimsCountForStageTypeBasedOnMonthAndYear for each stage and awaits results.
    // - Builds a list of DTOs representing the count per stage.
    public async Task<IEnumerable<ClaimStatusReportDTO>> ClaimStatusReportsBasedOnMonthAndYear(int month, int year)
    {

        List<ClaimStatusReportDTO> claims = new List<ClaimStatusReportDTO>();


        try
        {

            foreach (Stages e in Enum.GetValues(typeof(Stages)))
            {
                claims.Add(new ClaimStatusReportDTO
                {
                    Stage = e,
                    Count = await _claimDetailRepository.GetClaimsCountForStageTypeBasedOnMonthAndYear(e, month, year)
                });
            }
        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }



        return claims;

    }

    // PaymentStatusBasedOnMonthAndYear:
    // - Delegates to repository PaymentStatusOnMonthAndYear which uses SumAsync in DAL.
    public async Task<ClaimPaymentReportDTO> PaymentStatusBasedOnMonthAndYear(int month, int year)
    {

        int totalPayment;
        try
        {
            totalPayment = await _claimDetailRepository.PaymentStatusOnMonthAndYear(month, year);
        }
        catch (Exception ex)
        {
            //Log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return new ClaimPaymentReportDTO
        {
            Month = month,
            Year = year,
            Amount = totalPayment
        };
    }

    // AddNewClaim:
    // - Business validation sequence:
    //   1. Verify policy exists via PolicyService (await).
    //   2. Check policy.status and date constraints.
    //   3. Determine if a claim for the same policy exists in the same year (in-memory LINQ on policy.ClaimDetails).
    // - Uses AutoMapper to map request DTO to ClaimDetail entity.
    // - Calls AddNewClaim in repository (which performs model validation and persistence).
    public async Task<CommonOutput> AddNewClaim(ClaimDetailRequestDTO claimDetail)
    {

        
        CommonOutput result;
        try
        {

            CommonOutput policyOutput = await _policyService.GetPolicyByPolicyNo(null,claimDetail.PolicyNo);
            if (policyOutput.Result == RESULT.FAILURE)
            {
                if (policyOutput.Output == null)
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="PolicyNo",
                                ErrorMessage="Policy does not exist"
                            }
                        }
                    };
                }
                else
                {
                    //To write claim authorization error...
                    result=new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = policyOutput.Output
                    };
                }
            }
            else {
                Policy policy = (Policy)policyOutput.Output;
                if (policy.status == false){
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="PolicyNo",
                                ErrorMessage="Policy status is false"
                            }
                        }
                    };
                }
                else if (policy.DateOfInsurance >= claimDetail.DateOfAccident)
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="DateOfAccident",
                                ErrorMessage="DateOfAccident cannot be less than Policy DateOfInsurance"
                            }
                        }
                    };
                }
                else
                {
                    //ClaimDetail? prevClaimIfAny = await _claimDetailRepository.GetClaimByPolicyNo(claimDetail.PolicyNo);
                    // Using in-memory LINQ on the policy's ClaimDetails collection:
                    ClaimDetail? prevClaimIfAny = policy.ClaimDetails.Where(cd=>cd.DateOfAccident.Year==((DateOnly)claimDetail.DateOfAccident).Year).FirstOrDefault();
                    if (prevClaimIfAny == null)
                    {
                        ClaimDetail req = _mapper.Map<ClaimDetail>(claimDetail);
                        req.ClaimId = GenerateClaimID(claimDetail);
                        result = await _claimDetailRepository.AddNewClaim(req);

                        GetErrorListInRequiredFormat(ref result);

                    }
                    else
                    {
                        throw new MaximumClaimLimitReachedException("You cannot raise claim request for a policy number twice in a year.");
                    }


                }
            }
            
            
        }
        catch (Exception ex)
        {
            //Log error
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }

    // UpdateClaim:
    // - Fetch claim by id, validate it exists and is not closed.
    // - Optionally set SurveyorID (validates surveyor exists via SurveyorService).
    // - Updates ClaimStatus and InsuranceCompanyApproval where provided.
    // - Calls repository UpdateClaim to persist.
    public async Task<CommonOutput> UpdateClaim(string claimID, UpdateClaimDTO value)
    {
        CommonOutput result;
        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimID);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else if (claim.ClaimStatus == ClaimStatus.Closed)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimStatus",
                            ErrorMessage="Claim is already closed, so we cannot update the claim."
                        }
                    }
                };
            }
            else
            {

                if (value.SurveyorID != null)
                {


                    SurveyorDTO? surveyor = await _surveyorService.GetSurveyorById((int)value.SurveyorID);
                    if (surveyor == null)
                    {
                        result = new CommonOutput
                        {
                            Result = RESULT.FAILURE,
                            Output = new List<PropertyValidationResponse>
                            {
                                new PropertyValidationResponse
                                {
                                    Property="SurveyorId",
                                    ErrorMessage="Surveyor doesn't exist"
                                }
                            }
                        };
                        goto end_of_function;
                    }
                    else
                    {
                        claim.SurveyorID = (int)value.SurveyorID;
                    }
                }


                if (value.ClaimStatus != null)
                {
                    claim.ClaimStatus = (ClaimStatus)value.ClaimStatus;
                }
                if (value.InsuranceCompanyApproval != null)
                {
                    claim.InsuranceCompanyApproval = (bool)value.InsuranceCompanyApproval;
                    if ((bool)claim.InsuranceCompanyApproval == true)
                    {
                        claim.ClaimStatus = ClaimStatus.Closed;
                    }
                }

                result = await _claimDetailRepository.UpdateClaim(claim);





            }
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
    end_of_function:
        return result;
    }

    // UpdateClaimAmtApprovedBySurveyor:
    // - Validates claim existence and that a surveyor was assigned.
    // - Sets AmtApprovedBySurveyor and persists via repository.UpdateClaim.
    public async Task<CommonOutput> UpdateClaimAmtApprovedBySurveyor(string claimID, int claimant)
    {
        CommonOutput result;
        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimID);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else if (claim.SurveyorID == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="SurveyorId",
                            ErrorMessage="You cannot set the Amount approved when the surveyor is not assigned."
                        }
                    }
                };
            }
            else
            {
                claim.AmtApprovedBySurveyor = claimant;
                result = await _claimDetailRepository.UpdateClaim(claim);

                GetErrorListInRequiredFormat(ref result);
            }



        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;

    }

    // UpdateClaimSurveyorFees:
    // - Fetches Fee via IFeeService based on EstimatedLoss (which internally uses EF range query).
    // - Assigns fee value to claim and persists.
    // - If successful, populates result.Output with FeeDTO.
    public async Task<CommonOutput> UpdateClaimSurveyorFees(string claimID)
    {
        CommonOutput result;
        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimID);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else
            {
                if (claim.SurveyorID == null)
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="SurveyorID",
                                ErrorMessage="Surveyor not assigned"
                            }
                        }
                    };
                }
                else
                {
                    FeeDTO? fees = await _feeService.GetFeesByEstimatedLoss(claim.EstimatedLoss);


                    claim.SurveyorFees = fees.fees;
                    result = await _claimDetailRepository.UpdateClaim(claim);

                    if (result.Result == RESULT.SUCCESS)
                    {
                        result.Output = fees;
                    }
                    else
                    {
                        GetErrorListInRequiredFormat(ref result);
                    }


                }

            }


        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }

        //---New EndPoint--
    // UpdateAcceptRejectClaim:
    // - Accepts a boolean to mark withdraw status accepted/rejected and persists.
    // - Guard prevents withdrawing once WithdrawClaim==ACCEPTED and trying to set false.
    public async Task<CommonOutput> UpdateAcceptRejectClaim(string claimId,bool acceptReject){

        CommonOutput result;
        try{
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimId);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else if(claim.WithdrawClaim==WITHDRAWSTATUS.ACCEPTED && acceptReject==false){
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="WithdrawClaim",
                            ErrorMessage="You cannot withdraw a claim once it is accepted"
                        }
                    }
                };
            }
            else{
                if(acceptReject==true){
                    claim.WithdrawClaim=WITHDRAWSTATUS.ACCEPTED;
                }
                else{
                    claim.WithdrawClaim=WITHDRAWSTATUS.WITHDRAWN;
                }
                result=await _claimDetailRepository.UpdateClaim(claim);
            }   
            
        }
        catch (Exception ex){
            throw;
        }
        return result;

    } 


    //---Required-----
    public async Task<ClaimListOpenDTO?> GetClaimByClaimId(string claimId)
    {

        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimId);
            if (claim == null)
            {
                return null;
            }
            return _mapper.Map<ClaimListOpenDTO>(claim);

        }
        catch (Exception ex)
        {
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }

    }


    //----Not Required-----
    private async Task<CommonOutput> AssignSurveyorToClaim(string claimID)
    {
        CommonOutput result;
        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimID);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else
            {
                SurveyorDTO? surveyor = await _surveyorService.GetMinAllocatedSurveyorBasedOnEstimatedLoss(claim.EstimatedLoss);
                if (surveyor == null)
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="SurveyorId",
                                ErrorMessage="Surveyor doesn't exist"
                            }
                        }
                    };
                }
                else
                {
                    ClaimDetail temp = new ClaimDetail
                    {
                        ClaimId = claimID,
                        SurveyorID = surveyor.SurveyorId
                    };
                    result = await _claimDetailRepository.UpdateClaim(temp);
                }

            }


        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }

    private async Task<CommonOutput> AssignSurveyorToClaim(string claimID, int surveyorId)
    {
        CommonOutput result;
        try
        {
            ClaimDetail? claim = await _claimDetailRepository.GetClaimByClaimId(claimID);
            if (claim == null)
            {
                result = new CommonOutput
                {
                    Result = RESULT.FAILURE,
                    Output = new List<PropertyValidationResponse>
                    {
                        new PropertyValidationResponse
                        {
                            Property="ClaimId",
                            ErrorMessage="Claim ID doesn't exist"
                        }
                    }
                };
            }
            else
            {
                SurveyorDTO? surveyor = await _surveyorService.GetSurveyorById(surveyorId);
                if (surveyor == null)
                {
                    result = new CommonOutput
                    {
                        Result = RESULT.FAILURE,
                        Output = new List<PropertyValidationResponse>
                        {
                            new PropertyValidationResponse
                            {
                                Property="SurveyorId",
                                ErrorMessage="Surveyor doesn't exist"
                            }
                        }
                    };
                }
                else
                {
                    ClaimDetail temp = new ClaimDetail
                    {
                        ClaimId = claimID,
                        SurveyorID = surveyor.SurveyorId
                    };
                    result = await _claimDetailRepository.UpdateClaim(temp);
                }

            }


        }
        catch (Exception ex)
        {
            //log
            //_logger.Error(LogMessage(ex.Message));
            throw;
        }
        return result;
    }


    //--Helper functions--

    // GenerateClaimID:
    // - Simple deterministic generator using policy prefix + accident year.
    private string GenerateClaimID(ClaimDetailRequestDTO claimDetail)
    {
        string claimId = "CL";
        claimId += claimDetail.PolicyNo.Substring(0, 4);
        claimId += ((DateOnly)claimDetail.DateOfAccident).Year.ToString();

        return claimId;
    }


    // GetErrorListInRequiredFormat:
    // - Converts ValidationResult collection into a List<PropertyValidationResponse> for API-friendly output.
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


    private string LogMessage(string message)
    {
        return "Ran with this problem " + message + " in ClaimDetailService";
    }

}
