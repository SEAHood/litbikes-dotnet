using Nine.Geometry;
using System;
using System.Numerics;

namespace LitBikes.Util
{
    public class GeometryUtil
    {
        public static Vector2 GetLineOrientation(LineSegment2D line)
        {
            bool noXChange = line.Start.X == line.End.X;
            bool noYChange = line.Start.Y == line.End.Y;

            if (noXChange == noYChange)
                throw new Exception("There cannot be the same change in X and Y or something");

            return new Vector2(noXChange ? 0 : 1, noYChange ? 0 : 1);
        }
    }

    public static class LineSegment2DExtensions
    {
        public static bool Intersects(this LineSegment2D line, Rectangle rect)
        {
            var left = new LineSegment2D(new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height));
            var right = new LineSegment2D(new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height));
            var top = new LineSegment2D(new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y));
            var bottom = new LineSegment2D(new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X + rect.Width, rect.Y + rect.Height));

            return left.Intersects(line) ||
                   right.Intersects(line) ||
                   top.Intersects(line) ||
                   bottom.Intersects(line);
        }

    }
}
