namespace SharedModules;

public class InvalidMonthOrYearException : Exception
{

    public InvalidMonthOrYearException() { }
    public InvalidMonthOrYearException(string message):base(message) { }

}

public class MaximumClaimLimitReachedException : Exception
{

    public MaximumClaimLimitReachedException() { }

    public MaximumClaimLimitReachedException(string message):base(message) { }

}
