namespace SharedModules;

/// <summary>
/// This class is common output that is returned from various functions mainly add and update. It returns the success of the function in the Result variable.
/// The other property `Output` can contain any object that may be validation errors list or claimId or may be null.
/// </summary>
public class CommonOutput
{
    public RESULT Result { get; set; }
    public object? Output { get; set; }
}
