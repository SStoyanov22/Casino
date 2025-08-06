using Casino.Core.Enums;
using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface IValidationService
{
    ValidationResult ValidateAmount(CommandType commandType, decimal amount, decimal? balance = null);
}