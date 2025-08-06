namespace Casino.Infrastructure.Interfaces;

public interface IRandomNumberGeneratorService
{
    decimal GetRandomDecimal(decimal min, decimal max);
}