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
    private Mock<IValidationService> _mockValidationService;

    [SetUp]
    public void Setup()
    {
        _mockValidationService = new Mock<IValidationService>();
        _walletService = new WalletService(Mock.Of<ILogger<WalletService>>());
    }

    [Test]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var depositAmount = 50m;
        _mockValidationService.Setup(x => x.ValidateAmount(CommandType.Deposit, depositAmount, null))
            .Returns(ValidationResult.Error("Invalid amount"));

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
        _mockValidationService.Setup(x => x.ValidateAmount(CommandType.Deposit, depositAmount, null))
            .Returns(ValidationResult.Error("Invalid amount"));

        // Act
        var result = _walletService.Deposit(player, depositAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("Your deposit of $-10 has failed."));
    }

    [Test]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var withdrawAmount = 30m;
        _mockValidationService.Setup(x => x.ValidateAmount(CommandType.Withdraw, withdrawAmount, player.Wallet.Balance))
            .Returns(ValidationResult.Success());

        // Act
        var result = _walletService.Withdraw(player, withdrawAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(70)));
        Assert.That(result.Message, Does.Contain("Your withdrawal of $30 was succesful. Your current balance is $70"));
    }

    [Test]
    public void Withdraw_WithInsufficientFunds_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(20m);
        var withdrawAmount = 50m;
        _mockValidationService.Setup(x => x.ValidateAmount(CommandType.Withdraw, withdrawAmount, player.Wallet.Balance))
            .Returns(ValidationResult.Success());

        // Act
        var result = _walletService.Withdraw(player, withdrawAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(20)));
        Assert.That(result.Message, Does.Contain("Withdraw amount must be less than balance"));
    }

    [Test]
    public void PlaceBet_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var player = CreateTestPlayer(100);
        var betAmount = 10m;
        _mockValidationService.Setup(x => x.ValidateAmount(CommandType.Bet, betAmount, null))
            .Returns(ValidationResult.Success());

        // Act
        var result = _walletService.AcceptLoss(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(player.Wallet.Balance, Is.EqualTo(new Money(90)));
        Assert.That(result.Message, Does.Contain("No luck this time! Your current balance is: $90"));
    }

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
}