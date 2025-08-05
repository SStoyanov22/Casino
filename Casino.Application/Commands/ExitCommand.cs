using Casino.Core.Commands;
using Casino.Core.Constants;
using Casino.Core.DTOs;
using Casino.Core.Enums;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;

namespace Casino.Application.Commands;

public class ExitCommand : ICommand<CommandResult>
{
    private readonly ILogger<ExitCommand> _logger;
    public CommandType CommandType => CommandType.Exit;

    public ExitCommand(ILogger<ExitCommand> logger)
    {
        _logger = logger;
    }

    public Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation(LogMessages.CommandExecutionExit);

        return Task.FromResult(CommandResult.Success(UserMessages.Goodbye));
    }
}