using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class ValidationServiceTests : TestBase
{
    private ValidationService _validationService;
    private GameConfiguration _gameConfiguration;
    private Mock<IOptions<GameConfiguration>> _mockGameConfig;

    [SetUp]
    public void Setup()
    {
        _gameConfiguration = new GameConfiguration
        {
            MinimumBet = 1.0m,
            MaximumBet = 10.0m,
            LossProbability = 0.5m,
            SmallWinProbability = 0.4m,
            BigWinProbability = 0.1m,
            SmallWinMaxMultiplier = 2.0m,
            BigWinMinMultiplier = 2.0m,
            BigWinMaxMultiplier = 10.0m
        };

        _mockGameConfig = new Mock<IOptions<GameConfiguration>>();
        _mockGameConfig.Setup(x => x.Value).Returns(_gameConfiguration);

        _validationService = new ValidationService(_mockGameConfig.Object);
    }

    #region Deposit Validation Tests

    [Test]
    public void ValidateAmount_DepositWithValidAmount_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 50m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Deposit, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void ValidateAmount_DepositWithZeroAmount_ShouldReturnError()
    {
        // Arrange
        var amount = 0m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Deposit, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.DepositAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_DepositWithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var amount = -10m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Deposit, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.DepositAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_DepositWithSmallPositiveAmount_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 0.01m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Deposit, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_DepositWithLargeAmount_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 1000000m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Deposit, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    #endregion

    #region Withdraw Validation Tests

    [Test]
    public void ValidateAmount_WithdrawWithValidAmount_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 30m;
        var balance = 100m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void ValidateAmount_WithdrawWithZeroAmount_ShouldReturnError()
    {
        // Arrange
        var amount = 0m;
        var balance = 100m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.WithdrawAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_WithdrawWithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var amount = -5m;
        var balance = 100m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.WithdrawAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_WithdrawMoreThanBalance_ShouldReturnError()
    {
        // Arrange
        var amount = 150m;
        var balance = 100m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.WithdrawAmountMustBeLessThanBalance));
    }

    [Test]
    public void ValidateAmount_WithdrawExactBalance_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 100m;
        var balance = 100m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_WithdrawWithoutProvidingBalance_ShouldUseZeroBalance()
    {
        // Arrange
        var amount = 50m;
        // balance not provided, should default to 0

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.WithdrawAmountMustBeLessThanBalance));
    }

    [Test]
    public void ValidateAmount_WithdrawWithZeroBalance_ShouldReturnError()
    {
        // Arrange
        var amount = 1m;
        var balance = 0m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Withdraw, amount, balance);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.WithdrawAmountMustBeLessThanBalance));
    }

    #endregion

    #region Bet Validation Tests

    [Test]
    public void ValidateAmount_BetWithValidAmount_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 5m; // Within range [1, 10]

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void ValidateAmount_BetWithZeroAmount_ShouldReturnError()
    {
        // Arrange
        var amount = 0m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.BetAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_BetWithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var amount = -2m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.BetAmountMustBeGreaterThanZero));
    }

    [Test]
    public void ValidateAmount_BetBelowMinimum_ShouldReturnError()
    {
        // Arrange
        var amount = 0.5m; // Below minimum bet (1.0)

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(
            string.Format(UserMessages.BetAmountOutMustBeBetween, _gameConfiguration.MinimumBet, _gameConfiguration.MaximumBet)));
    }

    [Test]
    public void ValidateAmount_BetAboveMaximum_ShouldReturnError()
    {
        // Arrange
        var amount = 15m; // Above maximum bet (10.0)

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(
            string.Format(UserMessages.BetAmountOutMustBeBetween, _gameConfiguration.MinimumBet, _gameConfiguration.MaximumBet)));
    }

    [Test]
    public void ValidateAmount_BetAtMinimum_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 1.0m; // Exactly at minimum

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_BetAtMaximum_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 10.0m; // Exactly at maximum

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_BetJustAboveMinimum_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 1.01m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_BetJustBelowMaximum_ShouldReturnSuccess()
    {
        // Arrange
        var amount = 9.99m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    #endregion

    #region Unknown Command Type Tests

    [Test]
    public void ValidateAmount_WithUnknownCommandType_ShouldReturnError()
    {
        // Arrange
        var amount = 50m;
        var unknownCommandType = (CommandType)999; // Invalid enum value

        // Act
        var result = _validationService.ValidateAmount(unknownCommandType, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.UnknownOperation));
    }

    [Test]
    public void ValidateAmount_WithExitCommandType_ShouldReturnError()
    {
        // Arrange
        var amount = 50m;

        // Act
        var result = _validationService.ValidateAmount(CommandType.Exit, amount);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(UserMessages.UnknownOperation));
    }

    #endregion

    #region Edge Cases and Configuration Changes

    [Test]
    public void ValidateAmount_WithCustomGameConfiguration_ShouldUseNewLimits()
    {
        // Arrange
        var customConfig = new GameConfiguration
        {
            MinimumBet = 5.0m,
            MaximumBet = 20.0m
        };

        var mockCustomConfig = new Mock<IOptions<GameConfiguration>>();
        mockCustomConfig.Setup(x => x.Value).Returns(customConfig);

        var customValidationService = new ValidationService(mockCustomConfig.Object);

        // Act
        var resultTooLow = customValidationService.ValidateAmount(CommandType.Bet, 3m);
        var resultValid = customValidationService.ValidateAmount(CommandType.Bet, 10m);
        var resultTooHigh = customValidationService.ValidateAmount(CommandType.Bet, 25m);

        // Assert
        Assert.That(resultTooLow.IsValid, Is.False);
        Assert.That(resultValid.IsValid, Is.True);
        Assert.That(resultTooHigh.IsValid, Is.False);
    }

    [Test]
    public void ValidateAmount_WithDecimalPrecision_ShouldWork()
    {
        // Arrange
        var amount = 1.001m; // Very precise decimal

        // Act
        var result = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateAmount_WithVeryLargeNumber_ShouldHandleCorrectly()
    {
        // Arrange
        var amount = decimal.MaxValue;

        // Act
        var depositResult = _validationService.ValidateAmount(CommandType.Deposit, amount);
        var betResult = _validationService.ValidateAmount(CommandType.Bet, amount);

        // Assert
        Assert.That(depositResult.IsValid, Is.True);
        Assert.That(betResult.IsValid, Is.False); // Should be above maximum bet
    }

    #endregion
}
