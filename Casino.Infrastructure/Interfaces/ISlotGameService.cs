using Casino.Core.Configurations;
using Casino.Core.Entities;
using Casino.Core.Enums;
using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface ISlotGameService
{
    GameResultType DetermineGameResult(GameConfiguration config);
    decimal CalculateWinAmount(decimal betAmount, GameResultType gameResultType, GameConfiguration gameConfiguration);
    CommandResult ProcessBet(Player player, decimal betAmount);

}