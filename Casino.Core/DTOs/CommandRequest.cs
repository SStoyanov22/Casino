using Casino.Core.Entities;

namespace Casino.Core.DTOs;

public class CommandRequest
{
    public decimal Amount { get; set; }
    public Player Player { get; set; }

    public CommandRequest(decimal amount, Player player)
    {
        Amount = amount;
        Player = player;
    }
}