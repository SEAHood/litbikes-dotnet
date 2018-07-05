using System;
using System.Drawing;

namespace LitBikes.Util
{
    public class ColourUtil
    {
        private static readonly float MIN_BRIGHTNESS = 0.8f;
        public static Color getBrightColor()
        {
            var random = new Random();
            var h = random.NextDouble();
            var s = random.NextDouble();
            var b = MIN_BRIGHTNESS + ((1f - MIN_BRIGHTNESS) * random.NextDouble());
            //var c = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            var c = Color.Aqua; // TODO
            return c;
        }
    }
}
