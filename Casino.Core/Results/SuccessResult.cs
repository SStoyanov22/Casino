namespace Casino.Core.Results;

public class SuccessResult : Result
{
    public SuccessResult(string message, decimal? newBalance = null)
    {
        IsSuccess = true;
        Message = message;
        NewBalance = newBalance;
    }
}