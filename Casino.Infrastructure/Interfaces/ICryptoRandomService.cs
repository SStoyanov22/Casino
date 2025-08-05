namespace Casino.Infrastructure.Interfaces;

public interface ICryptoRandomService
{
    int GetRandomInt(int min, int max);
    decimal GetRandomDecimal(decimal min, decimal max);
}