 // Casino.Core/Constants/LogMessages.cs
namespace Casino.Core.Constants
{
    public static class LogMessages
    {
        // Application lifecycle
        public const string ApplicationStarting = "Casino Application Starting...";
        public const string ApplicationShutdown = "Casino shutdown";
        public const string FatalErrorOccurred = "Fatal error occurred";
        
        // Game engine
        public const string GameEngineStarting = "Casino game engine starting...";
        public const string GameEngineStarted = "Casino game engine started successfully";
        public const string GameEngineStopping = "Casino game engine stopping...";
        public const string GameEngineStopped = "Casino game engine stopped";
        
        // Application service
        public const string ApplicationServiceStarting = "Application service starting";
        public const string ApplicationServiceStopped = "Application service stopped";
        public const string UnexpectedErrorInMainLoop = "Unexpected error in main application loop";
        
        // Commands
        public const string CommandExecutionStarted = "Executing {0} command for player {PlayerId} with amount ${1:0.##}";
        public const string CommandExecutionCompleted = "{0} command completed for player {1} with new balance {2}";
        public const string CommandExecutionFailed = "{0} command failed for player {1}: {2}";
        public const string CommandExecutionExit = "Exit command executing";
        public const string CommandExecutionExitSuccess = "Exit command executed successfully";
        public const string CommandNotFound = "Command of type {0} not found";
        public const string ParsedCommandWithoutAmount = "Parsed command without amount - Command: {0}";
        public const string CommandRequiresAmount = "Command {0} requires an amount";
        public const string CommandInvalidAmountFormat = "Invalid amount format for command {0}: {1}";
        public const string InvalidCommand = "Invalid command.";

        // Wallet service
        public const string DepositSuccessful = "Deposit of ${0:0.##} was succesful. Current balance is ${1:0.##}";
        public const string DepositFailed = "Deposit of ${0:0.##} has failed.";
        public const string DepositUnexpectedError = "An unexpected error occurred during deposit of ${0:0.##}";
        public const string WithdrawSuccessful = "Withdrawal of ${0:0.##} was succesful. Current balance is ${1:0.##}";
        public const string WithdrawFailed = "Your withdrawal of ${0:0.##} has failed.";
        public const string WithdrawUnexpectedError = "An unexpected error occurred during withdrawal of ${0:0.##}";
        public const string AcceptLossSuccessful = "Accept Loss successful. New balance: ${0:0.##}";
        public const string AcceptLossFailed = "Accept Loss failed with amount ${0:0.##}";
        public const string AcceptLossUnexpectedError = "An unexpected error occurred during Accept Loss of ${0:0.##}";
        public const string AcceptWinSuccessful = "Accept Win successful. New balance: ${0:0.##}";
        public const string AcceptWinFailed = "Accept Win failed with amount ${0:0.##}";
        public const string AcceptWinUnexpectedError = "An unexpected error occurred during win of ${0:0.##}";

        //Input
        public const string InputEmpty = "Input is empty";
        public const string ErrorReadingUserInput = "Error reading user input";
        public const string EmptyInputAfterSplitting = "Empty input after splitting";
        public const string ParsedInput = "Parsed input - Command: {0}, Amount: ${1:0.##}";

        // Validation
        public const string ValidationStarted = "Starting validation for {0}";
        public const string ValidationPassed = "Validation passed for {0}";
        public const string ValidationFailed = "Validation failed for {0}: {1}";
        
        // Game operations
        public const string GameResultWin = "Player {0} won ${1:0.##} with bet ${2:0.##}";
        public const string GameResultLoss = "Player {0} lost bet ${1:0.##}";
        
        // Configuration
        public const string ConfigurationLoaded = "Configuration loaded successfully";
        public const string ConfigurationError = "Configuration error: {0}";
        
        // Service operations
        public const string ServiceStarted = "{0} service started";
        public const string ServiceStopped = "{0} service stopped";
        public const string ServiceError = "Error in {0} service: {Error}";

        // Money/Amount validation
        public const string AmountMustBePositive = "Amount must be positive";
        public const string MoneyAmountCannotBeNegative = "Money cannot be negative";

        public const string DepositAmountMustBeGreaterThanMinimumAllowed = "Deposit amount must be greater than minimum allowed";
        public const string DepositAmountMustBeLessThanMaximumAllowed = "Deposit amount must be less than maximum allowed";
        public const string WithdrawAmountMustBeGreaterThanZero = "Withdraw amount must be greater than zero";
        public const string WithdrawAmountMustBeLessThanBalance = "Withdraw amount must be less than balance";
        public const string WithdrawAmountMustBeLessThanMaximumAllowed = "Withdraw amount must be less than maximum allowed";
        public const string WithdrawAmountMustBeGreaterThanMinimumAllowed = "Withdraw amount must be greater than minimum allowed";
        public const string BetAmountMustBeGreaterThanZero = "Bet amount must be greater than zero";
        public const string BetAmountMustBeLessThanBalance = "Bet amount must be less than balance";
        public const string BetAmountOutOfRange = "Bet amount must be between ${0:0.##} and ${1:0.##}";
        // Wallet operations
        public const string InsufficientFunds = "Insufficient funds";
        public const string InvalidGameResultType = "Invalid game result type";
        
        // Validation
        public const string InvalidConfiguration = "Game configuration is invalid";
        public const string InvalidBetRange = "Bet amount is outside allowed range";
    }
}