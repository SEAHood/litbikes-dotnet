using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace LitBikes.Model
{
    public class TrailSegmentDto
    {
        public bool isHead;
        public Vector2 start;
        public Vector2 end;
    }

    public class BikeDto
    {
        public Vector2 pos;
        public Vector2 dir;
        public float spd;
        public List<TrailSegmentDto> trail;
        public String colour; // in rgba(0,0,0,%A%) format
    }

    public class Bike
    {
        //private static Logger LOG = Log.getLogger(Bike.class);	
        private readonly int ownerPid;
        private Vector2 pos;
        private Vector2 dir;
        private float spd;
        private Trail trail;
        private Vector2 startPos;
        private Color colour;
        private Random random = new Random();

        public Bike(int ownerPid)
        {
            this.ownerPid = ownerPid;
        }

        public void Init(Spawn spawn, bool newPlayer)
        {
            pos = spawn.GetPos();
            dir = spawn.GetDir();
            spd = spawn.GetSpd();
            trail = new Trail();
            startPos = pos;
            AddTrailPoint();

            if (newPlayer)
                colour = generateBikeColour();
        }

        private Color generateBikeColour()
        {
            String[] colours = {
            "#ff0099",
            "#b4ff69",
            "#69b4ff",
            "#ffb469",
            "#69ffb4",
            "#f3f315",
            "#f1c40f",
            "#e74c3c",
            "#8e44ad",
            "#3498db",
            "#2ecc71",
            "#9b59b6",
            "#27ae60",
            "#1abc9c",
            "#2980b9",
            "#d35400",
            "#f39c12"
        };
            String colour = colours[random.Next(colours.Length)];
            return Color.decode(colour);
        }

        public void UpdatePosition()
        {
            float xDiff = dir.X * spd;
            float yDiff = dir.Y * spd;
            pos = Vector2.Add(pos, new Vector2(xDiff, yDiff));
        }

        private void AddTrailPoint()
        {
            LineSegment2D segLine;
            if (trail.Size() > 0)
            {
                LineSegment2D lastSeg = trail.getHead().GetLine();
                segLine = new LineSegment2D();
                segLine.Start.X = lastSeg.End.X;
                segLine.Start.Y = lastSeg.End.Y;
                segLine.End.X = pos.X;
                segLine.End.Y = pos.Y;
            }
            else
            {
                segLine = new LineSegment2D();
                segLine.Start.X = pos.X;
                segLine.Start.Y = pos.Y;
                segLine.End.X = pos.X;
                segLine.End.Y = pos.Y;
            }
            trail.Add(new TrailSegment(ownerPid, segLine));
        }

        public bool Collides(List<TrailSegment> trail, int lookAhead)
        {
            if (trail == null) return false;

            float aheadX = pos.X + (lookAhead * dir.X);
            float aheadY = pos.Y + (lookAhead * dir.Y);

            LineSegment2D line = new LineSegment2D();
            line.Start.X = pos.X;
            line.Start.Y = pos.Y;
            line.End.X = aheadX;
            line.End.Y = aheadY;

            foreach (TrailSegment segment in trail)
            {
                if (line.Intersects(segment.GetLine()))
                    return true;
            }

            return false;
        }

        public BikeDto GetDto()
        {
            BikeDto dto = new BikeDto
            {
                pos = new Vector2(pos.X, pos.Y),
                dir = new Vector2(dir.X, dir.Y),
                spd = spd,
                trail = trail.GetList().Select(t => t.GetDto()).ToList(),
                colour = $"rgba({colour.getRed()},{colour.getGreen()},{colour.getBlue()},%A%)"
            };
            return dto;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void SetPos(Vector2 pos)
        {
            this.pos = pos;
        }

        public Point GetPosAsPoint()
        {
            return new Point((int)pos.X, (int)pos.Y);
        }

        public Vector2 GetDir()
        {
            return dir;
        }

        public double GetSpd()
        {
            return spd;
        }

        public void SetSpd(float spd)
        {
            this.spd = spd;
        }

        public void SetDir(Vector2 dir)
        {
            if ((this.dir.X == 0 && dir.X == 0) || (this.dir.Y == 0 && dir.Y == 0))
                return;

            this.dir = dir;
            AddTrailPoint();
        }

        public List<TrailSegment> GetTrailSegmentList(bool withHead)
        {
            if (!withHead)
                return trail.GetList();

            var trailWithHead = new List<TrailSegment>(trail.GetList());
            float headSegmentStartX = 0;
            float headSegmentStartY = 0;
            if (trail.Size() > 0)
            {
                var head = trail.getHead();
                if (head == null)
                {
                    return null;
                }
                var lastSeg = trail.getHead().GetLine();
                headSegmentStartX = lastSeg.End.X;
                headSegmentStartY = lastSeg.End.Y;
            }
            else
            {
                headSegmentStartX = startPos.x;
                headSegmentStartY = startPos.y;
            }

            foreach (var t in trailWithHead)
            {
                t.SetHead(false);
            }
            var headSegment = new TrailSegment(ownerPid, new LineSegment2D(new Vector2(headSegmentStartX, headSegmentStartY), new Vector2(pos.X, pos.Y)));
            headSegment.SetHead(true);
            trailWithHead.Add(headSegment);

            return trailWithHead;
        }

        public Color GetColour()
        {
            return colour;
        }

        public void Crash()
        {
            SetDir(Vector2.Zero);
            AddTrailPoint();
        }

        public override String ToString()
        {
            return "bike s(" + pos.X + ", " + pos.Y + "), s(" + dir.X + ", " + dir.Y + ")";
        }

        public void BreakTrailSegment(ImpactPoint impactPoint, double radius)
        {
            trail.BreakSegment(impactPoint, radius);
        }
    }
}
