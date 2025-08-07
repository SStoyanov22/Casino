using Casino.Core.Commands;
using Casino.Core.Constants;
using Casino.Core.DTOs;
using Casino.Core.Enums;
using Casino.Core.Results;
using Casino.Infrastructure.Services;
using Moq;

namespace Casino.Tests.Services;

[TestFixture]
public class CommandDispatcherTests : TestBase
{
    private CommandDispatcher _commandDispatcher;
    private Mock<ICommand<CommandResult>> _mockDepositCommand;
    private Mock<ICommand<CommandResult>> _mockWithdrawCommand;
    private Mock<ICommand<CommandResult>> _mockBetCommand;
    private Mock<ICommand<CommandResult>> _mockExitCommand;
    private List<ICommand<CommandResult>> _commands;

    [SetUp]
    public void Setup()
    {
        // Create mock commands
        _mockDepositCommand = new Mock<ICommand<CommandResult>>();
        _mockWithdrawCommand = new Mock<ICommand<CommandResult>>();
        _mockBetCommand = new Mock<ICommand<CommandResult>>();
        _mockExitCommand = new Mock<ICommand<CommandResult>>();

        // Setup command types
        _mockDepositCommand.Setup(x => x.CommandType).Returns(CommandType.Deposit);
        _mockWithdrawCommand.Setup(x => x.CommandType).Returns(CommandType.Withdraw);
        _mockBetCommand.Setup(x => x.CommandType).Returns(CommandType.Bet);
        _mockExitCommand.Setup(x => x.CommandType).Returns(CommandType.Exit);

        // Create commands collection
        _commands = new List<ICommand<CommandResult>>
        {
            _mockDepositCommand.Object,
            _mockWithdrawCommand.Object,
            _mockBetCommand.Object,
            _mockExitCommand.Object
        };

        _commandDispatcher = new CommandDispatcher(_commands);
    }

    #region DispatchAsync Tests

    [Test]
    public async Task DispatchAsync_WithDepositCommand_ShouldExecuteDepositCommand()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var request = new CommandRequest(50m, player);
        var expectedResult = CommandResult.Success("Deposit successful");

        _mockDepositCommand.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _commandDispatcher.DispatchAsync(CommandType.Deposit, request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task DispatchAsync_WithWithdrawCommand_ShouldExecuteWithdrawCommand()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var request = new CommandRequest(30m, player);
        var expectedResult = CommandResult.Success("Withdraw successful");

        _mockWithdrawCommand.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _commandDispatcher.DispatchAsync(CommandType.Withdraw, request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task DispatchAsync_WithBetCommand_ShouldExecuteBetCommand()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var request = new CommandRequest(5m, player);
        var expectedResult = CommandResult.Success("Bet placed successfully");

        _mockBetCommand.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _commandDispatcher.DispatchAsync(CommandType.Bet, request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task DispatchAsync_WithExitCommand_ShouldExecuteExitCommand()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var request = new CommandRequest(0m, player);
        var expectedResult = CommandResult.Success("Exit successful");

        _mockExitCommand.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _commandDispatcher.DispatchAsync(CommandType.Exit, request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task DispatchAsync_WithUnknownCommand_ShouldReturnError()
    {
        // Arrange
        var player = CreateTestPlayer(100m);
        var request = new CommandRequest(50m, player);
        var unknownCommandType = (CommandType)999; // Invalid command type

        // Act
        var result = await _commandDispatcher.DispatchAsync(unknownCommandType, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task DispatchAsync_WhenCommandExecutionFails_ShouldReturnFailureResult()
    {
        // Arrange
        var player = CreateTestPlayer(10m);
        var request = new CommandRequest(50m, player); // Insufficient funds
        var expectedErrorResult = CommandResult.Error("Insufficient funds");

        _mockWithdrawCommand.Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedErrorResult);

        // Act
        var result = await _commandDispatcher.DispatchAsync(CommandType.Withdraw, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("Insufficient funds"));

    }
    #endregion

    #region GetAvailableCommands Tests

    [Test]
    public void GetAvailableCommands_ShouldReturnAllRegisteredCommandTypes()
    {
        /// Act
        var availableCommands = _commandDispatcher.GetAvailableCommands().ToList();

        // Assert
        Assert.That(availableCommands, Contains.Item(CommandType.Deposit));
        Assert.That(availableCommands, Contains.Item(CommandType.Withdraw));
        Assert.That(availableCommands, Contains.Item(CommandType.Bet));
        Assert.That(availableCommands, Contains.Item(CommandType.Exit));
        Assert.That(availableCommands.Count, Is.EqualTo(4));
    }

    [Test]
    public void GetAvailableCommands_WithSingleCommand_ShouldReturnSingleCommandType()
    {
        // Arrange
        var singleCommand = new List<ICommand<CommandResult>> { _mockDepositCommand.Object };
        var singleCommandDispatcher = new CommandDispatcher(singleCommand);

        // Act
        var availableCommands = singleCommandDispatcher.GetAvailableCommands().ToList();

        // Assert
        Assert.That(availableCommands, Contains.Item(CommandType.Deposit));
        Assert.That(availableCommands.Count, Is.EqualTo(1));
    }

    #endregion

    #region Constructor Tests

    [Test]
    public void Constructor_WithNullCommands_ShouldNotThrow()
    {
        //Assert
        Assert.DoesNotThrow(() => new CommandDispatcher(null));
    }

    #endregion
}
