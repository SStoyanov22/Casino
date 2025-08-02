using Casino.Core.Enums;

namespace Casino.Core.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public TransactionType Type { get; }
    public decimal BalanceAfter { get; }
    public decimal Amount { get; }
    public DateTime Timestamp { get; }

    public Transaction(TransactionType type, decimal amount, decimal balanceAfter)
    {
        Id = Guid.NewGuid();
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Timestamp = DateTime.UtcNow;
    }
}