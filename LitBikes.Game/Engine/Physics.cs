using System.Collections.Generic;
using LitBikes.Model;
using Nine.Geometry;

namespace LitBikes.Game.Engine
{
    public class Physics
    {
        public static ImpactPoint FindClosestImpactPoint(Point origin, LineSegment2D ray, List<TrailSegment> segments)
        {
            var impactPoints = new List<ImpactPoint>();
            foreach (var seg in segments)
            {
                var p = GetLineIntersection(ray, seg.GetLine());
                if (p != null)
                    impactPoints.Add(new ImpactPoint(seg, p.Value));
            }

            ImpactPoint closestImpactPoint = null;
            double closestDistance = -1;
            foreach (var ip in impactPoints)
            {
                var line = new LineSegment2D(origin.ToVector2(), ip.GetPoint().ToVector2());
                var distance = line.Length();
                if (distance < closestDistance || closestDistance == -1)
                {
                    closestDistance = distance;
                    closestImpactPoint = ip;
                }
            }

            return closestImpactPoint;
        }

        public static Point? FindClosestIntersection(Point origin, LineSegment2D ray, List<LineSegment2D> lines)//, out Point intersection)
        {
            var points = new List<Point>();
            foreach (var line in lines)
            {
                var p = GetLineIntersection(ray, line);
                if (p != null)
                    points.Add(p.Value);
            }

            Point? closestPoint = null;
            double closestDistance = -1;
            foreach (var point in points)
            {
                var line = new LineSegment2D(origin.ToVector2(), point.ToVector2());
                double distance = line.Length();
                if (distance < closestDistance || closestDistance == -1)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        public static Point? GetLineIntersection(LineSegment2D line1, LineSegment2D line2)
        {
            return LineIntersect(
                line1.Start.X,
                line1.Start.Y,
                line1.End.X,
                line1.End.Y,
                line2.Start.X,
                line2.Start.Y,
                line2.End.X,
                line2.End.Y
            );
        }

        public static Point? LineIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            double denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (denom == 0.0)
            { // Lines are parallel.
                return null;
            }
            double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denom;
            double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denom;
            if (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
            {
                // Get the intersection point.
                return new Point((int)(x1 + ua * (x2 - x1)), (int)(y1 + ua * (y2 - y1)));
            }
            return null;
        }
    }
}
