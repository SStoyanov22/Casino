namespace Casino.Core.Entities;

public class Player 
{
    public Guid Id { get; set; }
    public Wallet Wallet { get; set; }
    public Player ()
    {
        Id = Guid.NewGuid();
        Wallet = new Wallet();
    }
}