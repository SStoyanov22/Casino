using Casino.Core.Commands;
using Casino.Core.DTOs;
using Casino.Core.Entities;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;

namespace Casino.Application.Commands;

public class ExitCommand : BaseCommand<CommandResult>
{
    public ExitCommand(ILogger logger) : base(logger)
    {
    }

    public override Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation("Exit Command Executing");

        return Task.FromResult(CommandResult.Success("Thank you for playing! Hope to see you again soon."));
    }
}