# Casino Wallet Console Application

A .NET Core console application that simulates a casino wallet system with deposit, withdrawal, and betting functionality.

## 🎯 Project Overview

This application demonstrates a fully functional casino wallet system with the following features:
- **Money deposit** - Players can deposit funds into their wallet
- **Money withdrawal** - Players can withdraw funds from their wallet
- **Betting system** - Players can place bets and play a simple slot game
- **Game logic** - Implements realistic casino game probabilities and payouts
- **Balance tracking** - Real-time balance updates after every operation

## 🎮 Game Rules

The casino game provides a simple betting experience with the following rules:
- **Bet Range**: $1 to $10 per bet
- **Win/Loss Probabilities** -  manually configured in `appsettings.json`:
  - 50% of bets lose
  - 40% of bets win up to 2x the bet amount
  - 10% of bets win between 2x and 10x the bet amount
- **Balance Calculation**: `{new balance} = {old balance} - {bet amount} + {win amount}`

## 🏗️ Architecture

The project follows Clean Architecture principles with the following structure:

```
Casino/
├── Casino.Wallet/          # Main console application
├── Casino.Core/            # Domain models and entities
├── Casino.Application/      # Business logic and commands
├── Casino.Infrastructure/  # External concerns (I/O, etc.)
├── Casino.Tests/           # Unit tests
└── Casino.IntegrationTests/ # Integration tests
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/Casino.git
   cd Casino
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   cd Casino.Wallet
   dotnet run
   ```

## 🎯 Usage

Once the application starts, you can use the following commands:

- `deposit <amount>` - Deposit money into your wallet
- `withdraw <amount>` - Withdraw money from your wallet
- `bet <amount>` - Place a bet and play the game
- `exit` - Exit the application

### Example Session
```
Please, submit action: deposit 10
Your deposit of $10 was successful. Your current balance is: $10

Please, submit action: bet 5
Congrats - you won $35.35! Your current balance is: $40.35

Please, submit action: withdraw 40
Your withdrawal of $40.00 was successful. Your current balance is: $0.35

Please, submit action: exit
Thank you for playing! Hope to see you again soon.
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test Casino.Tests
```

### Run Integration Tests
```bash
dotnet test Casino.IntegrationTests
```

## ⚙️ Configuration

The application uses `appsettings.json` for configuration:


## 🏗️ Technical Features

- **IoC Container** - Microsoft.Extensions.DependencyInjection
- **Command Pattern** - All operations implemented as commands
- **Integration Tests** - End-to-end testing
- **TODO** - TODO

## 📝 Development

### Project Structure
- **Casino.Core**: Domain entities, value objects, and interfaces
- **Casino.Application**: Business logic, commands, and application services
- **Casino.Infrastructure**: External concerns like I/O abstraction
- **Casino.Wallet**: Console application entry point
- **Casino.Tests**: Unit tests for all business logic
- **Casino.IntegrationTests**: Integration tests for complete workflows

### Key Design Patterns
- **TODO**: TODO

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature-name`)
3. Commit your changes (`git commit -m 'Adding some feature'`)
4. Push to the branch (`git push origin feature/my-feature-name`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built for educational purposes to demonstrate .NET Core best practices
- Implements casino game logic for entertainment purposes only
- No real money is involved in this application 