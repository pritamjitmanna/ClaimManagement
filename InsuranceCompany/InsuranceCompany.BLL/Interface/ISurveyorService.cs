using InsuranceCompany.BLL.RequestDTO;
using SharedModules;

namespace InsuranceCompany.BLL;

public interface ISurveyorService
{

    Task<IEnumerable<SurveyorDTO>> GetSurveyorListOnEstimatedLoss(int estimatedLoss);
    Task<SurveyorDTO?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int EstimatedLoss);

    Task<SurveyorDTO?> GetSurveyorById(string surveyorId);

    Task<CommonOutput> AddSurveyorDetails(SurveyorEntryDTO surveyorDTO);
    Task<bool> DeleteSurveyorDetails(string surveyorId);

}
