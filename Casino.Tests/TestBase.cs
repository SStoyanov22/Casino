using Moq;
using Microsoft.Extensions.Logging;
using Casino.Core.Entities;
using Casino.Core.ValueObjects;
namespace Casino.Tests;

public abstract class TestBase
{
    protected readonly Mock<ILogger> MockLogger;

    protected TestBase()
    {
        MockLogger = new Mock<ILogger>();
    }

    protected static Player CreateTestPlayer(decimal initialBalance = 0)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Wallet = new Wallet { Balance = new Money(initialBalance) }
        };
    }
}