using Casino.Core.Configurations;
using Casino.Core.Enums;

namespace Casino.Application.Services;

public interface ISlotGameService
{
    GameResultType DetermineGameResult(GameConfiguration config);
    decimal CalculateWinAmount(decimal betAmount, GameResultType gameResultType, GameConfiguration gameConfiguration);


}