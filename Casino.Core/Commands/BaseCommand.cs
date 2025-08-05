using Casino.Core.DTOs;
using Casino.Core.Entities;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;

namespace Casino.Core.Commands;

public abstract class BaseCommand<TResult> : ICommand<TResult> where TResult : IResult
{
    protected readonly ILogger _logger;

    protected BaseCommand(ILogger logger)
    {
        _logger = logger;
    }

    public abstract Task<CommandResult> ExecuteAsync(CommandRequest request);
}