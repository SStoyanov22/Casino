using Casino.Core.Enums;

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
            throw new InvalidOperationException(ExceptionMessages.);
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
            throw new ArgumentException("Bet amount must be greater than 0")
        }

        lock(_lock)
        {
            Balance += amount;
            _transactions.Add(new Transaction(TransactionType.Win, amount, Balance))
        }
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) 
        {
            throw new ArgumentException("Bet amount must be greater than 0")
        }

        if (amount > Balance)
        {
            throw new InvalidOperationException("Insufficient balance");
        }
        
        lock(_lock)
        {
            Balance -= amount;
            _transactions.Add(new Transaction(TransactionType.Withdrawal), amount, Balance)
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
        {
            throw new ArgumentException("Bet amount must be greater than 0")
        }

        lock(_lock)
        {
            Balance += amount;
            _transactions.Add(new Transaction(TransactionType.Deposit, amount, Balance))
        }
    }
}