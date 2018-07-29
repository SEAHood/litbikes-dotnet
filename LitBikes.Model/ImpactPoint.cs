using Nine.Geometry;
using System.Numerics;

namespace LitBikes.Model
{
    public class ImpactPoint
    {
        private TrailSegment trailSegment;
        private Point point;

        public ImpactPoint(TrailSegment seg, Point p)
        {
            trailSegment = seg;
            point = p;
        }

        public TrailSegment GetTrailSegment()
        {
            return trailSegment;
        }

        public void SetTrailSegment(TrailSegment trailSegment)
        {
            this.trailSegment = trailSegment;
        }

        public Point GetPoint()
        {
            return point;
        }

        public void SetPoint(Point point)
        {
            this.point = point;
        }
    }
}
