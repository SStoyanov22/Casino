using Casino.Core.Configurations;
using Casino.Core.Enums;

namespace Casino.Application.Services;

public interface IGameEngine
{
    GameResultType DetermineGameResult(GameConfiguration config);
    decimal CalculateWinAmount(decimal betAmount, GameResultType gameResultType, GameConfiguration gameConfiguration);


}