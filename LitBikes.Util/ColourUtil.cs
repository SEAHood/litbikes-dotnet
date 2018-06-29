using System;

namespace LitBikes.Util
{
    public class ColourUtil
    {
        private static readonly float MIN_BRIGHTNESS = 0.8f;
        public static Color getBrightColor()
        {
            Random random = new Random();
            double h = random.NextDouble();
            double s = random.NextDouble();
            double b = MIN_BRIGHTNESS + ((1f - MIN_BRIGHTNESS) * random.NextDouble());
            Color c = Color.getHSBColor(h, s, b);
            return c;
        }
    }
}
