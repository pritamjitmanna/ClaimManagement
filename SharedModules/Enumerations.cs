namespace SharedModules;

/// <summary>
/// It represents the Result of a function whether success or failure.
/// </summary>
public enum RESULT
{
    SUCCESS = 1,
    FAILURE = 0
}

/// <summary>
/// It represents the stages of a claim, whether New, Pending or Finalized
/// </summary>
public enum Stages
{
    NewClaims,
    PendingClaims,
    FinalizedClaims
}


/// <summary>
/// It represents the claim status whether Open or Closed.
/// </summary>
public enum ClaimStatus
{
    Open = 1,
    Closed = 0
}


public enum WITHDRAWSTATUS{
    NOSTATUS=0,
    ACCEPTED=1,
    WITHDRAWN=2
}
