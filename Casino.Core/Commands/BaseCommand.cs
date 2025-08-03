using Casino.Core.Results;
using Microsoft.Extensions.Logging;

namespace Casino.Core.Commands;

public abstract class BaseCommand<TResult> : ICommand<TResult> where TResult : IResult
{
    protected readonly ILogger Logger;

    protected BaseCommand(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<TResult> ExecuteAsync();
}