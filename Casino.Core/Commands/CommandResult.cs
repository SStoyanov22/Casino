using Casino.Core.Results;

namespace Casino.Core.Commands;

public class CommandResult : Result
{
    public static CommandResult Success(string message, decimal? newBalance = null)
    {
        return new CommandResult
        {
            IsSuccess = true,
            Message = message,
            NewBalance = newBalance
        };
    }

    public static CommandResult Error(string message)
    {
        return new CommandResult
        {
            IsSuccess = false,
            Message = message,
            NewBalance = null
        };
    }
}