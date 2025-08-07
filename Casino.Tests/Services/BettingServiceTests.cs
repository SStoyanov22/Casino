using Casino.Core.Configurations;
using Casino.Core.Entities;
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
public class BettingServiceTests : TestBase
{
    private BettingService _bettingService;
    private Mock<IWalletService> _mockWalletService;
    private Mock<ISlotGameService> _mockSlotGameService;
    private Mock<ILogger<BettingService>> _mockLogger;
    private Mock<IOptions<GameConfiguration>> _mockGameConfig;
    private GameConfiguration _gameConfiguration;

    [SetUp]
    public void Setup()
    {
        _mockWalletService = new Mock<IWalletService>();
        _mockSlotGameService = new Mock<ISlotGameService>();
        _mockLogger = new Mock<ILogger<BettingService>>();
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

        _bettingService = new BettingService(
            _mockLogger.Object,
            _mockWalletService.Object,
            _mockSlotGameService.Object,
            _mockGameConfig.Object);
    }

    #region Success Scenarios

    [Test]
    public void ProcessBet_WithSmallWin_ShouldReturnSuccessWithWinnings()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;
        var winAmount = 7.5m;
        
        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed!"))
            .Callback(() => player.Wallet.Balance = new Money(95m));
            
        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.SmallWin);
            
        _mockSlotGameService.Setup(x => x.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration))
            .Returns(winAmount);
               
        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Success("Accepted win!"))
            .Callback(() => player.Wallet.Balance = new Money(102.5m));
            
        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("Congrats - you won $7.5! Your current balance is: $102.5"));
    }

    [Test]
    public void ProcessBet_WithBigWin_ShouldReturnSuccessWithWinnings()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 10m;
        var winAmount = 50m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed!"))
            .Callback(() => player.Wallet.Balance = new Money(90m));

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.SmallWin);

        _mockSlotGameService.Setup(x => x.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration))
            .Returns(winAmount);

        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Success("Accepted win!"))
            .Callback(() => player.Wallet.Balance = new Money(140m));

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("Congrats - you won $50! Your current balance is: $140"));
    }

        [Test]
    public void ProcessBet_WithLoss_ShouldReturnSuccessWithLossMessage()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var betAmount = 5m;

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"))
            .Callback(() => player.Wallet.Balance = new Money(95m));

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.Loss);

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

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
        var result = _bettingService.ProcessBet(player, betAmount);

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
        var errorMessage = "Failed to accept win";

        _mockWalletService.Setup(x => x.PlaceBet(player, betAmount))
            .Returns(CommandResult.Success("Bet placed"));

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.SmallWin);

        _mockSlotGameService.Setup(x => x.CalculateWinAmount(betAmount, GameResultType.SmallWin, _gameConfiguration))
            .Returns(winAmount);

        _mockWalletService.Setup(x => x.AcceptWin(player, winAmount))
            .Returns(CommandResult.Error(errorMessage));

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

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

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Throws(new InvalidOperationException("Game service error"));

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while processing bet of $5"));
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
        var result = _bettingService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Does.Contain("An unexpected error occurred while processing bet of $5"));
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

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.Loss);

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

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

        _mockSlotGameService.Setup(x => x.DetermineGameResult(_gameConfiguration))
            .Returns(GameResultType.Loss);

        // Act
        var result = _bettingService.ProcessBet(player, betAmount);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    #endregion
}
