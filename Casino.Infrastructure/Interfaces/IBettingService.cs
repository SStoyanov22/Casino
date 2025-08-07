using Casino.Core.Entities;
using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface IBettingService
{
    CommandResult ProcessBet(Player player, decimal betAmount);
}