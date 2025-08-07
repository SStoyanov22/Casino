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

        // Use integer division to create inclusive range
        // Map [0, ulong.MaxValue] to [0.0, 1.0] inclusively
        var normalizedValue = randomValue / (double)ulong.MaxValue;

        // Adjust to ensure max can be reached
        if (randomValue == ulong.MaxValue)
            normalizedValue = 1.0;

        return min + ((decimal)normalizedValue * (max - min));
    }
}