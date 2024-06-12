using SharedModules;

namespace Surveyor.DAL;

public interface ISurveyor
{
    Task<SurveyReport?> GetSurveyReport(string claimId);
    Task<CommonOutput> AddSurveyReport(SurveyReport surveyReport);
    Task<CommonOutput> UpdateSurveyReport(SurveyReport surveyReport);
}
