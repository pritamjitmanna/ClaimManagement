using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace Surveyor.DAL;

public class SurveyorRepository:ISurveyor
{

    private readonly SurveyorDBContext _dbcontext;

    public SurveyorRepository(SurveyorDBContext dbcontext){
        _dbcontext = dbcontext;
    }


    public async Task<CommonOutput> AddSurveyReport(SurveyReport surveyReport){
        CommonOutput result;
        try{
            ICollection<ValidationResult>results=[];
            bool IsValid=ValidationFunctions.ValidateModel(surveyReport,ref results);

            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{

                await _dbcontext.Reports.AddAsync(surveyReport);
                await _dbcontext.SaveChangesAsync();
                result=new CommonOutput{
                    Result=RESULT.SUCCESS,
                    Output=surveyReport.ClaimId
                };
            }

        }
        catch(Exception ex){
            throw;
        }
        return result;
    }


    public async Task<SurveyReport?> GetSurveyReport(string claimId){
        try{
        SurveyReport? surveyReport = await _dbcontext.Reports.AsNoTracking().Where(r=>r.ClaimId==claimId).FirstOrDefaultAsync();
        return surveyReport;
        }
        catch(Exception ex){
            throw;
        }

    }

    public async Task<CommonOutput> UpdateSurveyReport(SurveyReport surveyReport){
        CommonOutput result;
        try{
            ICollection<ValidationResult>results=[];
            bool IsValid=ValidationFunctions.ValidateModel(surveyReport,ref results);

            if(!IsValid){
                result=new CommonOutput{
                    Result=RESULT.FAILURE,
                    Output=results
                };
            }
            else{

                _dbcontext.Reports.Update(surveyReport);
                await _dbcontext.SaveChangesAsync();
                result=new CommonOutput{
                    Result=RESULT.SUCCESS
                };
            }

        }
        catch(Exception ex){
            throw;
        }
        return result;
    }

}
