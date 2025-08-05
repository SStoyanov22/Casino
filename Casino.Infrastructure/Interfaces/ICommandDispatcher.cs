using Casino.Core.DTOs;
using Casino.Core.Enums;
using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface ICommandDispatcher
{
    Task<CommandResult> DispatchAsync(CommandType commandType, CommandRequest request);
    IEnumerable<CommandType> GetAvailableCommands();
}