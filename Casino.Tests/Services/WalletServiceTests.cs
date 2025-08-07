using Casino.Core.Enums;
using Casino.Core.Results;
using Casino.Core.ValueObjects;
using Casino.Infrastructure.Interfaces;
using Casino.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class WalletServiceTests : TestBase
{
    private WalletService _walletService;

    [SetUp]
    public void Setup()
    {
        _walletService = new WalletService(Mock.Of<ILogger<WalletService>>());
    }

    #region Deposit Tests

    [Test]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var depositAmount = 50m;

        // Act
        var result = _walletService.Deposit(player, depositAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(150m)));
        Assert.That(result.Message, Does.Contain("Your deposit of $50 was succesful. Your current balance is $150"));
    }

    [Test]
    public void Deposit_WithInvalidAmount_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var depositAmount = -10m;

        // Act
        var result = _walletService.Deposit(player, depositAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Amount cannot be negative! Your deposit of $-10 has failed."));
    }
    #endregion

    #region Withdraw Tests

    [Test]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var withdrawAmount = 30m;

        // Act
        var result = _walletService.Withdraw(player, withdrawAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(70)));
        Assert.That(result.Message, Does.Contain("Your withdrawal of $30 was succesful. Your current balance is $70"));
    }

    [Test]
    public void Withdraw_WithInvalidAmount_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(20m);
        var withdrawAmount = -50m;

        // Act
        var result = _walletService.Withdraw(player, withdrawAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(20)));
        Assert.That(result.Message, Does.Contain("Amount cannot be negative! Insufficient funds! Your withdrawal of $-50 has failed."));
    }

    [Test]
    public void Withdraw_WithInsufficientFunds_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(20m);
        var withdrawAmount = 50m;

        // Act
        var result = _walletService.Withdraw(player, withdrawAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(20)));
        Assert.That(result.Message, Does.Contain("Insufficient funds! Your withdrawal of $50 has failed."));
    }
    #endregion Withdraw Tests

    #region PlaceBet Tests

    [Test]
    public void PlaceBet_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var betAmount = 5m; // Valid bet amount (between 1 and 10)

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(95)));
        Assert.That(result.Message, Does.Contain("No luck this time! Your current balance is: $95"));
    }

    [Test]
    public void PlaceBet_WithInsufficientFunds_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(2m); // Less than minimum bet
        var betAmount = 5m; // Valid bet amount but exceeds balance

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(2m))); // Balance unchanged
        Assert.That(result.Message, Does.Contain("Insufficient funds! Place Bet failed with amount $5"));
    }

    [Test]
    public void PlaceBet_WithBetAmountTooLow_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var betAmount = 0.5m; // Below minimum bet (1.0)

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(100))); // Balance unchanged
        Assert.That(result.Message, Does.Contain("Bet amount must be between $1 and $10! Place Bet failed with amount $0.5"));
    }

    [Test]
    public void PlaceBet_WithBetAmountTooHigh_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var betAmount = 11m; // Above maximum bet (11.0)

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(100))); // Balance unchanged
        Assert.That(result.Message, Does.Contain("Bet amount must be between $1 and $10! Place Bet failed with amount $11."));
    }

    [Test]
    public void PlaceBet_WithExactMinimumBet_ShouldSucceed()
    {
        // Arrange
        var player = CreateTestPlayer(10);
        var betAmount = 1m; // Minimum bet amount

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(9)));
        Assert.That(result.Message, Does.Contain("No luck this time! Your current balance is: $9"));
    }

    [Test]
    public void PlaceBet_WithExactMaximumBet_ShouldSucceed()
    {
        // Arrange
        var player = CreateTestPlayer(20);
        var betAmount = 10m; // Maximum bet amount

        // Act
        var result = _walletService.PlaceBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(10)));
        Assert.That(result.Message, Does.Contain("No luck this time! Your current balance is: $10"));
    }

    #endregion

    #region AcceptWin Tests

    [Test]
    public void AcceptWin_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(50);
        var winAmount = 25m;

        // Act
        var result = _walletService.AcceptWin(player, winAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(75)));
        Assert.That(result.Message, Does.Contain("Congrats - you won $25! Your current balance is: $75"));
    }

    [Test]
    public void AcceptWin_WithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(50);
        var winAmount = -10m; // Negative win amount

        // Act
        var result = _walletService.AcceptWin(player, winAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(50))); // Balance unchanged
        Assert.That(result.Message, Does.Contain("Amount cannot be negative! Accept Win failed with amount $-10"));
    }

    [Test]
    public void AcceptWin_WithZeroAmount_ShouldSucceed()
    {
        // Arrange
        var player = CreateTestPlayer(50);
        var winAmount = 0m; // Zero win amount (valid)

        // Act
        var result = _walletService.AcceptWin(player, winAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(50))); // Balance unchanged but operation successful
        Assert.That(result.Message, Does.Contain("Congrats - you won $0! Your current balance is: $50"));
    }

    [Test]
    public void AcceptWin_WithLargeAmount_ShouldSucceed()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var winAmount = 1000m; // Large win amount

        // Act
        var result = _walletService.AcceptWin(player, winAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(1100)));
        Assert.That(result.Message, Does.Contain("Congrats - you won $1000! Your current balance is: $1100"));
    }

    [Test]
    public void AcceptWin_WithDecimalAmount_ShouldSucceed()
    {
        // Arrange
        var player = CreateTestPlayer(50);
        var winAmount = 12.75m; // Decimal win amount

        // Act
        var result = _walletService.AcceptWin(player, winAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(62.75m)));
        Assert.That(result.Message, Does.Contain("Congrats - you won $12.75! Your current balance is: $62.75"));
    }

    #endregion

    #region Helper Method Tests

    [Test]
    public void GetBalance_ShouldReturnCorrectBalance()
    {
        // Arrange
        var player = CreateTestPlayer(123.45m);

        // Act
        var balance = _walletService.GetBalance(player);

        // Assert
        Assert.That(balance, Is.EqualTo(123.45m));
    }

    [Test]
    public void HasSufficientFunds_WithSufficientBalance_ShouldReturnTrue()
    {
        // Arrange
        var player = CreateTestPlayer(100);

        // Act
        var result = _walletService.HasSufficientFunds(player, 50m);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasSufficientFunds_WithInsufficientBalance_ShouldReturnFalse()
    {
        // Arrange
        var player = CreateTestPlayer(30);

        // Act
        var result = _walletService.HasSufficientFunds(player, 50m);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void HasSufficientFunds_WithExactBalance_ShouldReturnTrue()
    {
        // Arrange
        var player = CreateTestPlayer(50);

        // Act
        var result = _walletService.HasSufficientFunds(player, 50m);

        // Assert
        Assert.That(result, Is.True);
    }

    #endregion
}