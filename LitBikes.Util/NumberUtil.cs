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
    }
}
