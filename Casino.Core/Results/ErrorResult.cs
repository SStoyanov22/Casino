namespace Casino.Core.Results;

public class ErrorResult : Result
{
    public ErrorResult(string message)
    {
        IsSuccess = false;
        Message = message;
        NewBalance = null;
    }
}