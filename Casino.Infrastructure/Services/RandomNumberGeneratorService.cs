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

        // Map [0, ulong.MaxValue] to (0.0, 1.0] - exclusive min, inclusive max
        var normalizedValue = (randomValue + 1.0) / ((double)ulong.MaxValue + 1.0);

        // Adjust to ensure max can be reached
        if (randomValue == ulong.MaxValue)
            normalizedValue = 1.0;

        return min + ((decimal)normalizedValue * (max - min));
    }
}