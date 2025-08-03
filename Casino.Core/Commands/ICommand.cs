using Casino.Core.Results;

namespace Casino.Core.Commands;

public interface ICommand<TResult> where TResult : IResult
{
    Task<TResult> ExecuteAsync();
}