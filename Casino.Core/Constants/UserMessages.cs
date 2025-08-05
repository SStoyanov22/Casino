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
        public const string AvailableCommands = @"Available commands: \n - deposit <amount> \n - withdraw <amount> \n - bet <amount> \n - exit";
        
        // Command results
        public const string DepositSuccessful = "Your deposit of {0:C} was succesful. Your current balance is {1:C}";
        public const string DepositFailed = "Your deposit of {0:C} has failed.";
        public const string DepositUnexpectedError = "An unexpected error occurred during deposit of {0:C}";
        public const string WithdrawSuccessful = "Your withdrawal of {0:C} was succesful. Your current balance is {1:C}";
        public const string WithdrawFailed = "Your withdrawal of {0:C} has failed.";
        public const string WithdrawUnexpectedError = "An unexpected error occurred during withdrawal of {0:C}";
        public const string BetWin = "Congrats - you won {0:C}! Your current balance is: {1:C}";
        public const string BetLoss = "No luck this time! Your current balance is: {0:C}";
        public const string BetFailed = "Bet failed with amount {0:C}";
        public const string BetUnexpectedError = "An unexpected error occurred during bet of {0:C}";
        public const string WinSuccessful = "Win successful. New balance: {0:C}";
        public const string WinFailed = "Win failed with amount {0}";
        public const string WinUnexpectedError = "An unexpected error occurred during win of {0:C}";
        
        // Errors
        public const string UnexpectedErrorInMainLoop = "Unexpected error in main application loop";
        
        // Validation errors
        public const string AmountMustBePositive = "Amount must be positive";
        public const string CommandCannotBeEmpty = "Command cannot be empty";
        public const string DepositAmountRequired = "Deposit amount is required";
        public const string WithdrawAmountRequired = "Withdrawal amount is required";
        public const string WithdrawAmountMustBeGreaterThanZero = "Withdraw amount must be greater than zero";
        public const string BetAmountRequired = "Bet amount is required";
        public const string BetAmountOutMustBeBetween = "Bet amount must be between {0:C} and {1:C}";
        public const string UnknownOperation = "Unknown operation";
        public const string WithdrawAmountMustBeLessThanBalance = "Withdraw amount must be less than balance";
        public const string DepositAmountMustBeGreaterThanZero = "Deposit amount must be greater than zero";
        public const string BetAmountMustBeGreaterThanZero = "Bet amount must be greater than zero";
        // Wallet operations
        public const string InvalidGameResultType = "Invalid game result type";
    
    }
}