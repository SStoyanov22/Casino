namespace Casino.Infrastructure.Interfaces;

public interface IRandomNumberGeneratorService
{
    int GetRandomInt(int min, int max);
    decimal GetRandomDecimal(decimal min, decimal max);
}