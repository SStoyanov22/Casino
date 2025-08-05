using Casino.Core.DTOs;
using Casino.Core.Entities;
using Casino.Core.Enums;
using Casino.Core.Results;

namespace Casino.Core.Commands;

public interface ICommand<TResult> where TResult : IResult
{
    CommandType CommandType { get; }
    Task<CommandResult> ExecuteAsync(CommandRequest request);
}