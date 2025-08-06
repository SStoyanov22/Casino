using System.Security.Cryptography;
using Casino.Infrastructure.Interfaces;

namespace Casino.Infrastructure.Services;

public class RandomNumberGeneratorService : IRandomNumberGeneratorService
{
    public decimal GetRandomDecimal(decimal min, decimal max)
    {
        var randomBytes = new byte[8];
        RandomNumberGenerator.Fill(randomBytes);
        var randomValue = BitConverter.ToUInt64(randomBytes, 0);
        var normalizedValue = (double)randomValue / ulong.MaxValue;
        
        return min + ((decimal)normalizedValue * (max - min));
    }
}