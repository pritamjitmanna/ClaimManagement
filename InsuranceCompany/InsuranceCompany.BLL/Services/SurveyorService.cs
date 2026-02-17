// Summary:
// Service layer for Surveyor-related operations. Uses the ISurveyor repository to fetch data
// and AutoMapper to convert DAL entities to DTOs. Methods are async and rely on 'await' to
// asynchronously wait for repository Task results. Exceptions are propagated after optional logging.

using System.ComponentModel.DataAnnotations;
using AutoMapper;
using InsuranceCompany.BLL.RequestDTO;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class SurveyorService : ISurveyorService
{
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    #pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used

    private readonly ISurveyor _surveyorRepository;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;

    public SurveyorService(ISurveyor surveyorRepository,IMapper mapper)
    {
        _surveyorRepository = surveyorRepository;
        _mapper = mapper;
        //_logger = logger;
    }

    // GetSurveyorListOnEstimatedLoss:
    // - Calls repository GetAllSurveyorsForEstimatedLoss which returns IEnumerable<Surveyor>.
    // - Uses AutoMapper to map each Surveyor entity to SurveyorDTO.
    // - Uses 'await' to asynchronously get repository results and foreach to iterate the collection.
    public async Task<IEnumerable<SurveyorDTO>> GetSurveyorListOnEstimatedLoss(int estimatedLoss)
    {

        List<SurveyorDTO> surveyors = new List<SurveyorDTO>();
        try
        {
            var result = await _surveyorRepository.GetAllSurveyorsForEstimatedLoss(estimatedLoss);
            foreach(var val in result)
            {
                // AutoMapper.Map<T> maps properties from source entity to DTO.
                surveyors.Add(_mapper.Map<SurveyorDTO>(val));
            }
        }
        catch (Exception ex)
        {
            //log
            //_logger.Error("Ran with this problem " + ex.Message + " in SurveyorService");
            throw;
        }

        return surveyors;
    }

    // GetMinAllocatedSurveyorBasedOnEstimatedLoss:
    // - Calls repository to obtain the best-fit surveyor (uses ordering in DAL).
    // - Maps the resulting Surveyor (or null) to SurveyorDTO.
    // - Note: mapping null returns null; maintain nullability.
    public async Task<SurveyorDTO?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int EstimatedLoss)
    {


        SurveyorDTO? surveyor;
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
        return surveyor;
    }

    // GetSurveyorById:
    // - Fetches a surveyor by primary key via repository and maps to DTO.
    // - Uses await to asynchronously wait for DB call.
    public async Task<SurveyorDTO?> GetSurveyorById(int surveyorId)
    {
        SurveyorDTO? surveyor;
        try
        {
            var temp = await _surveyorRepository.GetSurveyorById(surveyorId);
            surveyor = _mapper.Map<SurveyorDTO>(temp);
        }
        catch (Exception ex)
        {
            throw;
        }

        return surveyor;
    }

    public async Task<CommonOutput> AddSurveyorDetails(SurveyorEntryDTO surveyorDTO)
    {
        CommonOutput result;
        try{
            // Map DTO to DAL entity
            Surveyor surveyorEntity = _mapper.Map<Surveyor>(surveyorDTO);
            result= await _surveyorRepository.AddSurveyorDetails(surveyorEntity);
            GetErrorListInRequiredFormat(ref result);
        }
        catch(Exception ex)
        {
            throw;
        }
        return result;
    }

    public async Task<bool> DeleteSurveyorDetails(int surveyorId)
    {
        bool isDeleted=false;

        try
        {
            isDeleted=await _surveyorRepository.DeleteSurveyorDetails(surveyorId);
        }
        catch(Exception ex)
        {
            throw;
        }
        return isDeleted;

    }



    // ------------Helper functions----------

    private void GetErrorListInRequiredFormat(ref CommonOutput result)
    {
        if (result.Result == RESULT.FAILURE)
        {
            List<PropertyValidationResponse> validationErrors = new List<PropertyValidationResponse>();

            foreach (var err in (ICollection<ValidationResult>?)result.Output)
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

    #pragma warning restore CS8602 // Dereference of a possibly null reference.
    #pragma warning restore CS0168 // Variable is declared but never used
    #pragma warning restore IDE0059 // Unnecessary assignment of a value
}

