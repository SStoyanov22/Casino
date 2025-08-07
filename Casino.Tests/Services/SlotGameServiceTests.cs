using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Core.Results;
using Casino.Core.ValueObjects;
using Casino.Infrastructure.Interfaces;
using Casino.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class SlotGameServiceTests : TestBase
{
    private SlotGameService _slotGameService;
    private Mock<IRandomNumberGeneratorService> _mockRngService;
    private Mock<IWalletService> _mockWalletService;
     private Mock<ILogger<SlotGameService>> _mockLogger;
     private Mock<IOptions<GameConfiguration>> _mockGameConfig;
    private GameConfiguration _gameConfiguration;

    [SetUp]
    public void Setup()
    {
        _mockRngService = new Mock<IRandomNumberGeneratorService>();
        _mockWalletService = new Mock<IWalletService>();
        _mockLogger = new Mock<ILogger<SlotGameService>>();
        _mockGameConfig = new Mock<IOptions<GameConfiguration>>();

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
        
        _mockGameConfig.Setup(x => x.Value).Returns(_gameConfiguration);
        
        _slotGameService = new SlotGameService(
            _mockRngService.Object,
            _mockLogger.Object,
            _mockWalletService.Object,
            _mockGameConfig.Object
        );
    }

    #region DetermineGameResult Tests

    [Test]
    public void DetermineGameResult_WithRandomValueInLossRange_ShouldReturnLoss()
    {
        // Arrange
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.3m); // Within loss probability (0.5)

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.Loss));
    }

    [Test]
    public void DetermineGameResult_WithRandomValueInSmallWinRange_ShouldReturnSmallWin()
    {
        // Arrange
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.7m); // Within small win range (0.5 + 0.4 = 0.9)

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.SmallWin));
    }

    [Test]
    public void DetermineGameResult_WithRandomValueInBigWinRange_ShouldReturnBigWin()
    {
        // Arrange
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.95m); // Within big win range (0.9 to 1.0)

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.BigWin));
    }

    [Test]
    public void DetermineGameResult_WithExactLossBoundary_ShouldReturnSLoss()
    {
        // Arrange - exactly at loss boundary
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.5m); // Exactly at loss probability boundary

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.Loss));
    }

    [Test]
    public void DetermineGameResult_WithExactSmallWinBoundary_ShouldReturnBigWin()
    {
        // Arrange - exactly at small win boundary
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.9m); // Exactly at small win boundary (0.5 + 0.4)

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.SmallWin));
    }

    [Test]
    public void DetermineGameResult_WithMaximumValue_ShouldReturnBigWin()
    {
        // Arrange
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(1.0m);

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.BigWin));
    }

    [Test]
    public void DetermineGameResult_WithZeroValue_ShouldReturnLoss()
    {
        // Arrange
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.0m);

        // Act
        var result = _slotGameService.DetermineGameResult(_gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(GameResultType.Loss));
    }

    #endregion

    #region CalculateWinAmount Tests

    [Test]
    public void CalculateWinAmount_WithLoss_ShouldReturnZero()
    {
        // Arrange
        var betAmount = 5m;

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.Loss, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(0m));
    }

    [Test]
    public void CalculateWinAmount_WithSmallWin_ShouldReturnCorrectAmount()
    {
        // Arrange
        var betAmount = 10m;
        var multiplier = 1.5m;
        var expectedWinAmount = betAmount * multiplier;

        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, _gameConfiguration.SmallWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(expectedWinAmount));
    }

    [Test]
    public void CalculateWinAmount_WithBigWin_ShouldReturnCorrectAmount()
    {
        // Arrange
        var betAmount = 5m;
        var multiplier = 7.5m;
        var expectedWinAmount = betAmount * multiplier;

        _mockRngService.Setup(x => x.GetRandomDecimal(_gameConfiguration.BigWinMinMultiplier, _gameConfiguration.BigWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.BigWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(expectedWinAmount));
    }

    [Test]
    public void CalculateWinAmount_WithSmallWinMinimumMultiplier_ShouldReturnBetAmount()
    {
        // Arrange
        var betAmount = 8m;
        var multiplier = 1.0m; // Minimum multiplier for small win

        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, _gameConfiguration.SmallWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(betAmount)); // 1x multiplier = bet amount
    }

    [Test]
    public void CalculateWinAmount_WithBigWinMaximumMultiplier_ShouldReturnMaxAmount()
    {
        // Arrange
        var betAmount = 2m;
        var multiplier = 10.0m; // Maximum multiplier for big win

        _mockRngService.Setup(x => x.GetRandomDecimal(_gameConfiguration.BigWinMinMultiplier, _gameConfiguration.BigWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.BigWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(20m)); // 2 * 10 = 20
    }

    [Test]
    public void CalculateWinAmount_WithInvalidGameResultType_ShouldThrowException()
    {
        // Arrange
        var betAmount = 5m;
        var invalidGameResult = (GameResultType)999; // Invalid enum value

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _slotGameService.CalculateWinAmount(betAmount, invalidGameResult, _gameConfiguration));

        Assert.That(ex.Message, Is.EqualTo(UserMessages.InvalidGameResultType));
    }

    [Test]
    public void CalculateWinAmount_WithDecimalBetAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var betAmount = 2.5m;
        var multiplier = 3.2m;
        var expectedWinAmount = betAmount * multiplier; // 2.5 * 3.2 = 8.0

        _mockRngService.Setup(x => x.GetRandomDecimal(_gameConfiguration.BigWinMinMultiplier, _gameConfiguration.BigWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.BigWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(expectedWinAmount));
    }

    [Test]
    public void CalculateWinAmount_WithZeroBetAmount_ShouldReturnZero()
    {
        // Arrange
        var betAmount = 0m;
        var multiplier = 5m;

        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, _gameConfiguration.SmallWinMaxMultiplier))
            .Returns(multiplier);

        // Act
        var result = _slotGameService.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration);

        // Assert
        Assert.That(result, Is.EqualTo(0m)); // 0 * multiplier = 0
    }

    #endregion
    
    #region Success Scenarios

    [Test]
    public void ProcessBet_WithSmallWin_ShouldReturnSuccessWithWinnings()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;
        var multiplier = 1.5m;
        var winAmount = 7.5m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.7m);

        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, _gameConfiguration.SmallWinMaxMultiplier))
            .Returns(multiplier);

        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Success("Accepted win"));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Does.Contain("Congrats - you won"));
        Assert.That(result.Message, Does.Contain("Your current balance is"));
    }

    [Test]
    public void ProcessBet_WithBigWin_ShouldReturnSuccessWithWinnings()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 10m;
        var winAmount = 50m;
        var randomValue = 0.95m; // Big win range
        var multiplier = 5m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(randomValue);

        _mockRngService.Setup(x => x.GetRandomDecimal(_gameConfiguration.BigWinMinMultiplier, _gameConfiguration.BigWinMaxMultiplier))
            .Returns(multiplier);

        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Success("Accepted win"));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Does.Contain("Congrats - you won"));
        Assert.That(result.Message, Does.Contain("Your current balance is"));
    }

    [Test]
    public void ProcessBet_WithLoss_ShouldReturnSuccessWithLossMessage()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;
        var randomValue = 0.3m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(randomValue);

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Does.Contain("No luck this time"));
    }

    #endregion

    #region Error Scenarios

    [Test]
    public void ProcessBet_WhenPlaceBetFails_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(2m);
        var betAmount = 5m;
        var errorMessage = "Insufficient funds";

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Error(errorMessage));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public void ProcessBet_WhenAcceptWinFails_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;
        var winAmount = 10m;
        var randomValue = 0.7m; // Small win range
        var multiplier = 2m;
        var errorMessage = "Failed to accept win";

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(randomValue);

        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, _gameConfiguration.SmallWinMaxMultiplier))
            .Returns(multiplier);

        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Error(errorMessage));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public void ProcessBet_WhenSlotGameServiceThrows_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Throws(new InvalidOperationException("Game service error"));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while processing bet"));
    }

    [Test]
    public void ProcessBet_WhenWalletServiceThrows_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Throws(new InvalidOperationException("Wallet service error"));

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while processing bet"));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ProcessBet_WithMinimumBet_ShouldWork()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 1m; // Minimum bet

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.3m);

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ProcessBet_WithMaximumBet_ShouldWork()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 10m; // Maximum bet

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(0.3m);

        // Act
        var result = _slotGameService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    #endregion
}
