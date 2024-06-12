﻿namespace InsuranceCompany.BLL;

public interface ISurveyorService
{

    Task<IEnumerable<SurveyorDTO>> GetSurveyorListOnEstimatedLoss(int estimatedLoss);
    Task<SurveyorDTO?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int EstimatedLoss);

    Task<SurveyorDTO?> GetSurveyorById(int surveyorId);

}
