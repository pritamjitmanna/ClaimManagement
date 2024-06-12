using AutoMapper;
using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;

public class SurveyorService : ISurveyorService
{

    private readonly ISurveyor _surveyorRepository;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;

    public SurveyorService(ISurveyor surveyorRepository,IMapper mapper)
    {
        _surveyorRepository = surveyorRepository;
        _mapper = mapper;
        //_logger = logger;
    }

    public async Task<IEnumerable<SurveyorDTO>> GetSurveyorListOnEstimatedLoss(int estimatedLoss)
    {

        List<SurveyorDTO> surveyors = new List<SurveyorDTO>();

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            var result = await _surveyorRepository.GetAllSurveyorsForEstimatedLoss(estimatedLoss);
            foreach(var val in result)
            {
                surveyors.Add(_mapper.Map<SurveyorDTO>(val));
            }
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorService");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        return surveyors;
    }

    public async Task<SurveyorDTO?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int EstimatedLoss)
    {


        SurveyorDTO? surveyor;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            var temp = await _surveyorRepository.GetMinAllocatedSurveyorBasedOnEstimatedLoss(EstimatedLoss);
            surveyor=_mapper.Map<SurveyorDTO>(temp);
        }
        catch (Exception ex)
        {
            //log
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        return surveyor;
    }

    public async Task<SurveyorDTO?> GetSurveyorById(int surveyorId)
    {
        SurveyorDTO? surveyor;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            var temp = await _surveyorRepository.GetSurveyorById(surveyorId);
            surveyor = _mapper.Map<SurveyorDTO>(temp);
        }
        catch (Exception ex)
        {
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        return surveyor;
    }
}

