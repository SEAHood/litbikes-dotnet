using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Numerics;
using LitBikes.Model.Dtos;

namespace LitBikes.Model
{
    public class TrailSegment
    {
        private readonly string _id;
	    private readonly Guid _ownerPid;
        private readonly LineSegment2D _line;
	    private readonly Vector2 _orientation;

        public TrailSegment(Guid ownerPid, LineSegment2D line) : this(Guid.NewGuid().ToString(), ownerPid, line, GeometryUtil.GetLineOrientation(line)) { }

        private TrailSegment(string id, Guid ownerPid, LineSegment2D line, Vector2 orientation)
        {
            _id = id;
            _ownerPid = ownerPid;
            _line = line;
            _orientation = orientation;
        }

        public string GetId()
        {
            return _id;
        }

        public Guid GetOwnerPid()
        {
            return _ownerPid;
        }

        public LineSegment2D GetLine()
        {
            return _line;
        }

        public Vector2 GetOrientation()
        {
            return _orientation;
        }
        
        public TrailSegmentDto GetDto(bool isHead)
        {
            return new TrailSegmentDto
            {
                IsHead = isHead,
                Start = new Vector2(_line.Start.X, _line.Start.Y),
                End = new Vector2(_line.End.X, _line.End.Y)
            };
        }

        public TrailSegment Clone()
        {
            return new TrailSegment(_id, _ownerPid, _line, _orientation);
        }
    }
}
