// Casino.Core/Constants/UserMessages.cs
namespace Casino.Core.Constants
{
    public static class UserMessages
    {
        // Welcome and instructions
        public const string Welcome = "Welcome to our Casino!";
        public const string Goodbye = "Thank you for playing! Hope to see you again soon.";
        public const string PromptAction = "Please, submit action: ";
        public const string PleaseEnterValidCommand = "Please enter a valid command.";
        
        // Available commands
        public const string AvailableCommands = @"Available commands: deposit <amount> | withdraw <amount> | bet <amount> | exit";
        
        // Command results
        public const string DepositSuccessful = "Your deposit of ${0:0.##} was succesful. Your current balance is ${1:0.##}";
        public const string DepositFailed = "Your deposit of ${0:0.##} has failed.";
        public const string DepositUnexpectedError = "An unexpected error occurred during deposit of ${0:0.##}";
        public const string WithdrawSuccessful = "Your withdrawal of ${0:0.##} was succesful. Your current balance is ${1:0.##}";
        public const string WithdrawFailed = "Insufficient funds! Your withdrawal of ${0:0.##} has failed.";
        public const string WithdrawUnexpectedError = "An unexpected error occurred during withdrawal of ${0:0.##}";
        public const string NoLuck = "No luck this time! Your current balance is: ${0:0.##}";
        public const string PlaceBetFailed = "Place Bet failed with amount ${0:0.##}.";
        public const string PlaceBetSuccessful = "Place Bet successful. Your new balance is: ${0:0.##}";
        public const string PlaceBetUnexpectedError = "An unexpected error occurred during Place Bet of ${0:0.##}";
        public const string AcceptWinSuccessful = "Congrats - you won ${0:0.##}! Your current balance is: ${1:0.##}";
        public const string AcceptWinFailed = "Accept Win failed with amount ${0:0.##}.";
        public const string AcceptWinUnexpectedError = "An unexpected error occurred during win of ${0:0.##}";

        // Validation errors
        public const string WithdrawAmountMustBeGreaterThanZero = "Withdraw amount must be greater than zero";
        public const string BetAmountOutMustBeBetween = "Bet amount must be between ${0:0.##} and ${1:0.##}!";
        public const string UnknownOperation = "Unknown operation";
        public const string WithdrawAmountMustBeLessThanBalance = "Withdraw amount must be less than balance";
        public const string DepositAmountMustBeGreaterThanZero = "Deposit amount must be greater than zero";
        public const string BetAmountMustBeGreaterThanZero = "Bet amount must be greater than zero";
        
        // Wallet operations
        public const string InvalidGameResultType = "Invalid game result type";

        public const string UnexpectedErrorProcessingBet = "An unexpected error occurred while processing bet of ${0:0.##} for player {1}";

    }
}