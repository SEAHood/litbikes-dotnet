using System;

namespace LitBikes.Util
{
    public class NumberUtil
    {
        private static readonly Random _rand = new Random();

        public static int RandInt(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public static float RandFloat(float min, float max)
        {
            return (float)_rand.NextDouble() * (max - min) + min;
        }
    }
}
