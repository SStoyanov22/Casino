using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Infrastructure.Interfaces;
using Casino.Infrastructure.Services;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class SlotGameServiceTests : TestBase
{
    private SlotGameService _slotGameService;
    private Mock<IRandomNumberGeneratorService> _mockRngService;
    private GameConfiguration _gameConfiguration;

    [SetUp]
    public void Setup()
    {
        _mockRngService = new Mock<IRandomNumberGeneratorService>();
        _slotGameService = new SlotGameService(_mockRngService.Object);

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
    public void DetermineGameResult_WithExactLossBoundary_ShouldReturnSmallWin()
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

    #region Integration Tests

    [Test]
    public void SlotGameService_WithMultipleGameResults_ShouldProduceExpectedDistribution()
    {
        // Arrange
        var betAmount = 5m;
        var gameResults = new List<GameResultType>();
        var winAmounts = new List<decimal>();

        // Setup different random values for game determination
        var randomSequence = new Queue<decimal>(new[] { 0.3m, 0.7m, 0.95m }); // Loss, SmallWin, BigWin
        _mockRngService.Setup(x => x.GetRandomDecimal(0, 1))
            .Returns(() => randomSequence.Dequeue());

        // Setup win multipliers
        _mockRngService.Setup(x => x.GetRandomDecimal(1.0m, 2.0m))
            .Returns(1.5m);
        _mockRngService.Setup(x => x.GetRandomDecimal(2.0m, 10.0m))
            .Returns(5.0m);

        // Act
        for (int i = 0; i < 3; i++)
        {
            var gameResult = _slotGameService.DetermineGameResult(_gameConfiguration);
            var winAmount = _slotGameService.CalculateWinAmount(betAmount, gameResult, _gameConfiguration);
            
            gameResults.Add(gameResult);
            winAmounts.Add(winAmount);
        }

        // Assert
        Assert.That(gameResults[0], Is.EqualTo(GameResultType.Loss));
        Assert.That(winAmounts[0], Is.EqualTo(0m));

        Assert.That(gameResults[1], Is.EqualTo(GameResultType.SmallWin));
        Assert.That(winAmounts[1], Is.EqualTo(7.5m)); // 5 * 1.5

        Assert.That(gameResults[2], Is.EqualTo(GameResultType.BigWin));
        Assert.That(winAmounts[2], Is.EqualTo(25m)); // 5 * 5.0
    }

    #endregion
}
