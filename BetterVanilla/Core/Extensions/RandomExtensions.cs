using System;

namespace BetterVanilla.Core.Extensions;

public static class RandomExtensions
{
    extension(Random random)
    {
        public float Next(float minValue, float maxValue)
        {
            return (float) (random.NextDouble() * (maxValue - minValue) + minValue);
        }
    }
}