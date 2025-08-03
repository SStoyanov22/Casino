using Casino.Core.Commands;
using Microsoft.Extensions.Logging;

namespace Casino.Application.Commands;

public class ExitCommand : BaseCommand<CommandResult>
{
    public ExitCommand(ILogger logger) : base(logger)
    {
    }

    public override Task<CommandResult> ExecuteAsync()
    {
        Logger.LogInformation("Executing ExitCommand");
        return Task.FromResult(CommandResult.Success("Thank you for playing! Hope to see you again soon."));
    }
}