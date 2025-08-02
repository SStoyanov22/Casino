using Casino.Core.Enums;
using Casino.Core.Constants;
                
namespace Casino.Core.Entities;

public class Wallet
{
    public decimal Balance { get; private set; }
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();
    private readonly List<Transaction> _transactions = new ();
    private readonly object _lock = new();

    public void PlaceBet(decimal amount) 
    {
        if (amount <= 0)
        {
            throw new ArgumentException(ExceptionMessages.AmountMustBePositive);
        }

        if (amount > Balance)
        {
            throw new InvalidOperationException(ExceptionMessages.InsufficientFunds);
        }

        lock(_lock) 
        {
            Balance -= amount;
            _transactions.Add(new Transaction(TransactionType.Bet, amount, Balance));
        }
    }

    public void AcceptWin(decimal amount)
    {
        if (amount <= 0) 
        {
            throw new ArgumentException(ExceptionMessages.AmountMustBePositive);
        }

        lock(_lock)
        {
            Balance += amount;
            _transactions.Add(new Transaction(TransactionType.Win, amount, Balance));
        }
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) 
        {
            throw new ArgumentException(ExceptionMessages.AmountMustBePositive);
        }

        if (amount > Balance)
        {
            throw new InvalidOperationException(ExceptionMessages.InsufficientFunds);
        }
        
        lock(_lock)
        {
            Balance -= amount;
            _transactions.Add(new Transaction(TransactionType.Withdrawal, amount, Balance));
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
        {
            throw new ArgumentException(ExceptionMessages.AmountMustBePositive);
        }

        lock(_lock)
        {
            Balance += amount;
            _transactions.Add(new Transaction(TransactionType.Deposit, amount, Balance));
        }
    }
}