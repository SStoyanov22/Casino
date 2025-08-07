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
        public const string GameEngineStopping = "Casino game engine stopping...";
        
        // Commands
        public const string CommandExecutionStarted = "Executing {0} command for player {1} with amount ${2:0.##}";
        public const string CommandExecutionCompleted = "{0} command completed for player {1} with new balance {2:0.##}";
        public const string CommandExecutionFailed = "{0} command failed for player {1}: {2}";
        public const string CommandExecutionExit = "Exit command executing";
        public const string CommandNotFound = "Command of type {0} not found";
        public const string ParsedInputWithOneArgument = "Parsed input without amount - Command: {0}";
        public const string CommandInvalidAmountFormat = "Invalid amount format for command {0}: {1}";
        public const string InvalidCommand = "Invalid command: {0}";
        public const string CommandWithoutAmount = "Command {0} does not require an amount";

        // Wallet service
        public const string DepositSuccessful = "Deposit of ${0:0.##} was succesful. Current balance is ${1:0.##}";
        public const string DepositFailed = "Deposit of ${0:0.##} has failed.";
        public const string DepositUnexpectedError = "An unexpected error occurred during deposit of ${0:0.##}";
        public const string WithdrawSuccessful = "Withdrawal of ${0:0.##} was succesful. Current balance is ${1:0.##}";
        public const string WithdrawFailed = "Your withdrawal of ${0:0.##} has failed.";
        public const string WithdrawUnexpectedError = "An unexpected error occurred during withdrawal of ${0:0.##}";
        public const string PlaceBetSuccessful = "Place Bet successful. New balance: ${0:0.##}";
        public const string PlaceBetFailed = "Place Bet failed with amount ${0:0.##}";
        public const string PlaceBetUnexpectedError = "An unexpected error occurred during Place Bet of ${0:0.##}";
        public const string AcceptWinSuccessful = "Accept Win successful. New balance: ${0:0.##}";
        public const string AcceptWinFailed = "Accept Win failed with amount ${0:0.##}";
        public const string AcceptWinUnexpectedError = "An unexpected error occurred during win of ${0:0.##}";

        //Input
        public const string InputEmpty = "Input is empty";
        public const string InputTooLong = "Input is has too many arguments.";
        public const string ErrorReadingUserInput = "Error reading user input";
        public const string EmptyInputAfterSplitting = "Empty input after splitting";
        public const string ParsedInput = "Parsed input - Command: {0}, Amount: {1}";

        // Money/Amount validation
        public const string AmountCannotBeNegative = "Amount cannot be negative!";

        // Wallet operations
        public const string InsufficientFunds = "Insufficient funds!";

        // Betting service
        public const string ProcessingBet = "Processing bet of ${0:0.##} for player {1}";
        public const string FailedToPlaceBet = "Failed to place bet of ${0:0.##} for player {1}: {2}";
        public const string GameResultDetermined = "Game result determined: {0} for player {1}";
        public const string GameResultWin = "Game result is a win for player {0}";
        public const string GameResultLoss = "Game result is a loss for player {0}";
        public const string WinAmountCalculated = "Win amount calculated: ${0:0.##} for player {1}";
        public const string FailedToAcceptWin = "Failed to accept win of ${0:0.##} for player {1}: {2}";
        public const string WinProcessedSuccessfully = "Win processed successfully. Player {0} final balance: {1:0.##}";
        public const string UnexpectedErrorProcessingBet = "An unexpected error occurred while processing bet of ${0:0.##}";
    }
}