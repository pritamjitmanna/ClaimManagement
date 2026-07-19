using SharedModules;
using Surveyor.DAL;

namespace Surveyor.BLL;

public interface ISurveyorService
{
    Task<ReportDTO?> GetSurveyReport(string token,string claimId);
    
    Task<CommonOutput> AddNewSurveyReport(string token,SurveyReportDTO surveyReport);
    Task<CommonOutput> UpdateSurveyReport(string token,string claimId,UpdateReportDTO updateReportDTO);
}
