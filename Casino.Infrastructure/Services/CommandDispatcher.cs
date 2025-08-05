using Casino.Core.Results;
using Casino.Core.Enums;
using Casino.Core.Commands;
using Casino.Core.DTOs;
using Casino.Core.Constants;
using Casino.Infrastructure.Interfaces;

namespace Casino.Infrastructure.Services;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IEnumerable<ICommand<CommandResult>> _commands;
    
    public CommandDispatcher(IEnumerable<ICommand<CommandResult>> commands) 
    {
        _commands = commands; 
    }
    public async Task<CommandResult> DispatchAsync(CommandType commandType, CommandRequest request)
    {
        var command = _commands.FirstOrDefault(c => c.CommandType == commandType);
        
        if (command == null)
        {
            return CommandResult.Error(string.Format(LogMessages.CommandNotFound, commandType));
        }

        return await command.ExecuteAsync(request);
    }

    public IEnumerable<CommandType> GetAvailableCommands()
    {
        return _commands.Select(c => c.CommandType);
    }
}