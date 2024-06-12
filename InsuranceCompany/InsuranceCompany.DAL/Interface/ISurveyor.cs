namespace InsuranceCompany.DAL;

/// <summary>
/// This interface helps in creating the Surveyor repository for dealing with the Surveyor table.
/// </summary>
public interface ISurveyor
{

    /// <summary>
    /// This function returns the Surveyor who is the minimum allocated one among the list of Surveyors for the given estimated loss. It may return Null.
    /// </summary>
    /// <param name="estimatedLoss"></param>
    /// <returns>Surveyor?</returns>
    Task<Surveyor?> GetMinAllocatedSurveyorBasedOnEstimatedLoss(int estimatedLoss);

    /// <summary>
    /// This functions returns the list of all surveyors for the given estimatedLoss.
    /// </summary>
    /// <param name="estimatedLoss"></param>
    /// <returns>IEnumerable<Surveyor></returns>
    Task<IEnumerable<Surveyor>> GetAllSurveyorsForEstimatedLoss(int estimatedLoss);

    /// <summary>
    /// This function returns the Surveyor for a given surveyorId. It may return NULL.
    /// </summary>
    /// <param name="surveyorId"></param>
    /// <returns>Surveyor?</returns>
    Task<Surveyor?> GetSurveyorById(int surveyorId);

}
