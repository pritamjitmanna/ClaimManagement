using SharedModules;
using Surveyor.DAL;

namespace Surveyor.BLL;

public interface ISurveyorService
{
    Task<ReportDTO?> GetSurveyReport(string claimId);
    
    Task<CommonOutput> AddNewSurveyReport(SurveyReportDTO surveyReport);
    Task<CommonOutput> UpdateSurveyReport(string claimId,UpdateReportDTO updateReportDTO);
}
