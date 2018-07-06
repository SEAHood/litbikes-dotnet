using Nine.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LitBikes.Model
{
    public class Trail
    {
        private readonly ConcurrentDictionary<string, TrailSegment> segments;

        public Trail()
        {
            segments = new ConcurrentDictionary<string, TrailSegment>();
        }

        public List<TrailSegment> GetList()
        {
            var copy = new List<TrailSegment>();
            foreach (TrailSegment s in segments.Values)
            {
                copy.Add(s.Clone());
            }
            return copy;
        }

        public void Add(TrailSegment segment)
        {
            foreach (var s in segments.Values)
            {
                s.SetHead(false);
            }
            segment.SetHead(true);
            segments.TryAdd(segment.GetId(), segment);
        }

        public int Size()
        {
            return segments.Values.Count;
        }

        public TrailSegment GetHead()
        {
            foreach (var s in segments.Values)
            {
                if (s.IsHead())
                    return s;
            }
            return null;
        }

        public void ReplaceSegments(TrailSegment s, params TrailSegment[] newSegments)
        {
            var segment = segments.GetValueOrDefault(s.GetId());
            if (segment == null)
                return;

            // TODO this is clearly not done
        }

        public void BreakSegment(ImpactPoint impactPoint, double radius)
        {
            var requestedSegment = impactPoint.GetTrailSegment();
            var orientation = requestedSegment.GetOrientation();
            TrailSegment newSegment1 = null;
            TrailSegment newSegment2 = null;
            if (orientation.X == 1)
            {
                // segment is horizontal
                var y = requestedSegment.GetLine().Start.Y;
                var x1 = requestedSegment.GetLine().Start.X;
                var x2 = requestedSegment.GetLine().End.X;
                float t1x1, t1x2, t2x1, t2x2;
                if (x1 < x2)
                {
                    t1x1 = Math.Min(x1, x2);
                    t1x2 = (float)Math.Max(impactPoint.GetPoint().X - radius, t1x1);
                    t2x2 = Math.Max(x1, x2);
                    t2x1 = (float)Math.Min(impactPoint.GetPoint().X + radius, t2x2);
                }
                else
                {
                    t1x1 = Math.Max(x1, x2);
                    t1x2 = (float)Math.Min(impactPoint.GetPoint().X + radius, t1x1);
                    t2x2 = Math.Min(x1, x2);
                    t2x1 = (float)Math.Max(impactPoint.GetPoint().X - radius, t2x2);
                }

                var newSegment1Line = new LineSegment2D();
                newSegment1Line.Start.X = t1x1;
                newSegment1Line.Start.Y = y;
                newSegment1Line.End.X = t1x2;
                newSegment1Line.End.Y = y;

                var newSegment2Line = new LineSegment2D();
                newSegment2Line.Start.X = t2x1;
                newSegment2Line.Start.Y = y;
                newSegment2Line.End.X = t2x2;
                newSegment2Line.End.Y = y;

                newSegment1 = new TrailSegment(requestedSegment.GetOwnerPid(), newSegment1Line);
                newSegment2 = new TrailSegment(requestedSegment.GetOwnerPid(), newSegment2Line);
            }
            else if (orientation.Y == 1)
            {
                // segment is vertical
                var x = requestedSegment.GetLine().Start.X;
                var y1 = requestedSegment.GetLine().Start.Y;
                var y2 = requestedSegment.GetLine().End.Y;
                float t1y1, t1y2, t2y1, t2y2;
                if (y1 < y2)
                {
                    t1y1 = Math.Min(y1, y2);
                    t1y2 = (float)Math.Max(impactPoint.GetPoint().Y - radius, t1y1);
                    t2y2 = Math.Max(y1, y2);
                    t2y1 = (float)Math.Min(impactPoint.GetPoint().Y + radius, t2y2);
                }
                else
                {
                    t1y1 = Math.Max(y1, y2);
                    t1y2 = (float)Math.Min(impactPoint.GetPoint().Y + radius, t1y1);
                    t2y2 = Math.Min(y1, y2);
                    t2y1 = (float)Math.Max(impactPoint.GetPoint().Y - radius, t2y2);
                }

                var newSegment1Line = new LineSegment2D();
                newSegment1Line.Start.X = x;
                newSegment1Line.Start.Y = t1y1;
                newSegment1Line.End.X = x;
                newSegment1Line.End.Y = t1y2;

                var newSegment2Line = new LineSegment2D();
                newSegment2Line.Start.X = x;
                newSegment2Line.Start.Y = t2y1;
                newSegment2Line.End.X = x;
                newSegment2Line.End.Y = t2y2;

                newSegment1 = new TrailSegment(requestedSegment.GetOwnerPid(), newSegment1Line);
                newSegment2 = new TrailSegment(requestedSegment.GetOwnerPid(), newSegment2Line);
            }

            segments.TryGetValue(requestedSegment.GetId(), out var segment);
            if (segment == null)
            {
                if (requestedSegment.IsHead())
                {
                    segment = requestedSegment; // Segment being broken is (missing) segment between bike and last corner
                }
                else
                {
                    return;
                }
            }            

            // Int casts here may be an issue TODO
            var frontOfSegment = new Point((int)segment.GetLine().End.X, (int)segment.GetLine().End.Y);
            var newSegment1Point1 = new Point((int)newSegment1.GetLine().Start.X, (int)newSegment1.GetLine().Start.Y);
            var newSegment1Point2 = new Point((int)newSegment1.GetLine().End.X, (int)newSegment1.GetLine().End.Y);
            var newSegment2Point1 = new Point((int)newSegment2.GetLine().Start.X, (int)newSegment2.GetLine().Start.Y);
            var newSegment2Point2 = new Point((int)newSegment2.GetLine().End.X, (int)newSegment2.GetLine().End.Y);

            if (segment.IsHead())
            {
                foreach (var s in segments.Values)
                {
                    s.SetHead(false);
                }

                if (frontOfSegment.Equals(newSegment1Point1) || frontOfSegment.Equals(newSegment1Point2))
                {
                    newSegment1.SetHead(true);
                }
                else if (frontOfSegment.Equals(newSegment2Point1) || frontOfSegment.Equals(newSegment2Point2))
                {
                    newSegment2.SetHead(true);
                }
            }

            segments.Remove(requestedSegment.GetId(), out var _);
            segments.TryAdd(newSegment1.GetId(), newSegment1);
            segments.TryAdd(newSegment2.GetId(), newSegment2);
        }
    }
}
