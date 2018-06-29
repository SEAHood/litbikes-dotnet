using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Numerics;

namespace LitBikes.Model
{
    public class TrailSegment
    {
        private readonly String id;
	    private readonly int ownerPid;
        private readonly LineSegment2D line;
	    private readonly Vector2 orientation;
	    private bool isHead;

        public TrailSegment(int ownerPid, LineSegment2D line) :
            this(Guid.NewGuid().ToString(), ownerPid, line, GeometryUtil.GetLineOrientation(line), false) { }

        private TrailSegment(String id, int ownerPid, LineSegment2D line, Vector2 orientation, bool isHead)
        {
            this.id = id;
            this.ownerPid = ownerPid;
            this.line = line;
            this.orientation = orientation;
            this.isHead = isHead;
        }

        public String GetId()
        {
            return id;
        }

        public int GetOwnerPid()
        {
            return ownerPid;
        }

        public LineSegment2D GetLine()
        {
            return line;
        }

        public Vector2 GetOrientation()
        {
            return orientation;
        }

        public bool IsHead()
        {
            return isHead;
        }

        public void SetHead(bool isHead)
        {
            this.isHead = isHead;
        }

        public TrailSegmentDto GetDto()
        {
            return new TrailSegmentDto
            {
                isHead = isHead,
                start = new Vector2(line.Start.X, line.Start.Y),
                end = new Vector2(line.End.X, line.End.Y)
            };
        }

        public TrailSegment Clone()
        {
            return new TrailSegment(id, ownerPid, line, orientation, isHead);
        }
    }
}
