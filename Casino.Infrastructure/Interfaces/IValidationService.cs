using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface IValidationService
{
    ValidationResult ValidateDepositAmount(decimal amount);
    ValidationResult ValidateWithdrawAmount(decimal amount, decimal balance);
    ValidationResult ValidateBetAmount(decimal amount);
    ValidationResult ValidateAmount(decimal amount, string operations);
}