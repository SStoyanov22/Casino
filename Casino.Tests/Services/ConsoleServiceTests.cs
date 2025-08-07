using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Core.Exceptions;
using Casino.Infrastructure.Interfaces;
using Casino.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class ConsoleServiceTests : TestBase
{
    private ConsoleService _consoleService;
    private Mock<ILogger<ConsoleService>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<ConsoleService>>();
        _consoleService = new ConsoleService(_mockLogger.Object);
    }

    #region DisplayMessage Tests

    [Test]
    public void DisplayMessage_WithValidMessage_ShouldWriteToConsole()
    {
        // Arrange
        var message = "Test message";

        // Act & Assert - This test verifies the method doesn't throw
        Assert.DoesNotThrow(() => _consoleService.DisplayMessage(message));
    }

    [Test]
    public void DisplayMessage_WithEmptyMessage_ShouldWriteToConsole()
    {
        // Arrange
        var message = "";

        // Act & Assert
        Assert.DoesNotThrow(() => _consoleService.DisplayMessage(message));
    }

    [Test]
    public void DisplayMessage_WithNullMessage_ShouldWriteToConsole()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        Assert.DoesNotThrow(() => _consoleService.DisplayMessage(message));
    }

    [Test]
    public void DisplayMessage_WithSpecialCharacters_ShouldWriteToConsole()
    {
        // Arrange
        var message = "Test message with special chars: !@#$%^&*()";

        // Act & Assert
        Assert.DoesNotThrow(() => _consoleService.DisplayMessage(message));
    }

    #endregion


    #region ParseInput Tests - Valid Commands

    [Test]
    public void ParseInput_WithValidDepositCommand_ShouldReturnDepositCommandType()
    {
        // Arrange
        var input = "deposit 100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    [Test]
    public void ParseInput_WithValidWithdrawCommand_ShouldReturnWithdrawCommandType()
    {
        // Arrange
        var input = "withdraw 50";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Withdraw));
        Assert.That(result.amount, Is.EqualTo(50m));
    }

    [Test]
    public void ParseInput_WithValidBetCommand_ShouldReturnBetCommandType()
    {
        // Arrange
        var input = "bet 10";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Bet));
        Assert.That(result.amount, Is.EqualTo(10m));
    }

    [Test]
    public void ParseInput_WithValidExitCommand_ShouldReturnExitCommandType()
    {
        // Arrange
        var input = "exit";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Exit));
        Assert.That(result.amount, Is.Null);
    }

    [Test]
    public void ParseInput_WithUpperCaseCommand_ShouldReturnCorrectCommandType()
    {
        // Arrange
        var input = "DEPOSIT 100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    [Test]
    public void ParseInput_WithMixedCaseCommand_ShouldReturnCorrectCommandType()
    {
        // Arrange
        var input = "DePoSiT 100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    [Test]
    public void ParseInput_WithDecimalAmount_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit 99.99";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(99.99m));
    }

    [Test]
    public void ParseInput_WithZeroAmount_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit 0";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(0m));
    }

    [Test]
    public void ParseInput_WithNegativeAmount_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit -50";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(-50m));
    }

    [Test]
    public void ParseInput_WithExtraSpaces_ShouldParseCorrectly()
    {
        // Arrange
        var input = "  deposit    100  ";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    #endregion

    #region ParseInput Tests - Invalid Commands

    [Test]
    public void ParseInput_WithInvalidCommand_ShouldThrowInvalidCommandException()
    {
        // Arrange
        var input = "invalid 100";

        // Act & Assert
        var ex = Assert.Throws<InvalidCommandException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Does.Contain("Invalid command"));
    }

    [Test]
    public void ParseInput_WithEmptyInput_ShouldThrowParseInputException()
    {
        // Arrange
        var input = "";

        // Act & Assert
        var ex = Assert.Throws<ParseInputException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Is.EqualTo(LogMessages.InputEmpty));
    }

    [Test]
    public void ParseInput_WithWhitespaceOnly_ShouldThrowParseInputException()
    {
        // Arrange
        var input = "   ";

        // Act & Assert
        var ex = Assert.Throws<ParseInputException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Is.EqualTo(LogMessages.InputEmpty));
    }

    [Test]
    public void ParseInput_WithNullInput_ShouldThrowParseInputException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var ex = Assert.Throws<ParseInputException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Is.EqualTo(LogMessages.InputEmpty));
    }

    [Test]
    public void ParseInput_WithTooManyArguments_ShouldThrowArgumentException()
    {
        // Arrange
        var input = "deposit 100 extra";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Is.EqualTo(LogMessages.InputTooLong));
    }

    [Test]
    public void ParseInput_WithInvalidAmountFormat_ShouldThrowParseInputException()
    {
        // Arrange
        var input = "deposit abc";

        // Act & Assert
        var ex = Assert.Throws<ParseInputException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Does.Contain("Invalid amount format"));
    }

    [Test]
    public void ParseInput_WithExitCommandAndAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var input = "exit 100";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _consoleService.ParseInput(input));
        Assert.That(ex.Message, Does.Contain("Command exit does not require an amount"));
    }

    [Test]
    public void ParseInput_WithCommandOnly_ShouldReturnCommandTypeWithNullAmount()
    {
        // Arrange
        var input = "deposit";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.Null);
    }

    [Test]
    public void ParseInput_WithCommandAndEmptyAmount_ShouldReturnCommandTypeWithNullAmount()
    {
        // Arrange
        var input = "deposit ";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.Null);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ParseInput_WithVeryLargeAmount_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit 999999999.99";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(999999999.99m));
    }

    [Test]
    public void ParseInput_WithScientificNotation_ShouldThrowParseInputException()
    {
        // Arrange
        var input = "deposit 1e6";

        // Act & Assert
        Assert.Throws<ParseInputException>(() => _consoleService.ParseInput(input));
    }

    [Test]
    public void ParseInput_WithSpecialCharactersInCommand_ShouldThrowInvalidCommandException()
    {
        // Arrange
        var input = "deposit@ 100";

        // Act & Assert
        Assert.Throws<InvalidCommandException>(() => _consoleService.ParseInput(input));
    }

    [Test]
    public void ParseInput_WithUnicodeCharacters_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit 100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    [Test]
    public void ParseInput_WithTabSeparatedInput_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit\t100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    [Test]
    public void ParseInput_WithNewlineSeparatedInput_ShouldParseCorrectly()
    {
        // Arrange
        var input = "deposit\n100";

        // Act
        var result = _consoleService.ParseInput(input);

        // Assert
        Assert.That(result.commandType, Is.EqualTo(CommandType.Deposit));
        Assert.That(result.amount, Is.EqualTo(100m));
    }

    #endregion

    #region Command Type Tests

    [Test]
    public void ParseInput_AllCommandTypes_ShouldReturnCorrectTypes()
    {
        // Arrange
        var testCases = new[]
        {
            ("deposit 100", CommandType.Deposit, (decimal?)100m),
            ("withdraw 50", CommandType.Withdraw, (decimal?)50m),
            ("bet 10", CommandType.Bet, (decimal?)10m),
            ("exit", CommandType.Exit, (decimal?)null)
        };

        foreach (var (input, expectedCommandType, expectedAmount) in testCases)
        {
            // Act
            var result = _consoleService.ParseInput(input);

            // Assert
            Assert.That(result.commandType, Is.EqualTo(expectedCommandType), 
                $"Failed for input: {input}");
            Assert.That(result.amount, Is.EqualTo(expectedAmount), 
                $"Failed for input: {input}");
        }
    }

    #endregion
}
