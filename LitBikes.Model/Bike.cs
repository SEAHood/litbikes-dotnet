using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using LitBikes.Model.Dtos;
using Point = Nine.Geometry.Point;

namespace LitBikes.Model
{
    public class Bike
    {
        //private static Logger LOG = Log.getLogger(Bike.class);	
        private readonly Guid _ownerPid;
        private Vector2 _pos;
        private Vector2 _dir;
        private float _spd;
        private Trail _trail;
        private Vector2 _startPos;
        private Color _colour;
        private readonly Random _random = new Random();

        public Bike(Guid ownerPid)
        {
            _ownerPid = ownerPid;
        }

        public void Init(Spawn spawn, bool newPlayer)
        {
            _pos = spawn.GetPos();
            _dir = spawn.GetDir();
            _spd = spawn.GetSpd();
            _trail = new Trail();
            _startPos = _pos;
            AddTrailPoint();

            if (newPlayer)
                _colour = GenerateBikeColour();
        }

        private Color GenerateBikeColour()
        {
            string[] colours = {
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
            var colourHex = colours[_random.Next(colours.Length)];
            var colourConverter = new ColorConverter();
            var newColour = (Color?)colourConverter.ConvertFromString(colourHex);

            return newColour ?? Color.Red;
        }

        public void UpdatePosition()
        {
            var xDiff = _dir.X * _spd;
            var yDiff = _dir.Y * _spd;
            _pos = Vector2.Add(_pos, new Vector2(xDiff, yDiff));
        }

        private void AddTrailPoint()
        {
            LineSegment2D segLine;
            if (_trail.Size() > 0)
            {
                var lastSeg = _trail.GetHead().GetLine();
                segLine = new LineSegment2D
                {
                    Start = {
                        X = lastSeg.End.X,
                        Y = lastSeg.End.Y
                    },
                    End = {
                        X = _pos.X,
                        Y = _pos.Y
                    }
                };
            }
            else
            {
                segLine = new LineSegment2D
                {
                    Start = {
                        X = _pos.X,
                        Y = _pos.Y
                    },
                    End = {
                        X = _pos.X,
                        Y = _pos.Y
                    }
                };
            }
            _trail.Add(new TrailSegment(_ownerPid, segLine));
        }

        public bool Collides(List<TrailSegment> trail, int lookAhead, out Guid? collidedWith)
        {
            collidedWith = null;
            if (trail == null) return false;

            var aheadX = _pos.X + (lookAhead * _dir.X);
            var aheadY = _pos.Y + (lookAhead * _dir.Y);

            var line = new LineSegment2D
            {
                Start = {
                    X = _pos.X,
                    Y = _pos.Y
                },
                End = {
                    X = aheadX,
                    Y = aheadY
                }
            };

            foreach (var segment in trail)
            {
                if (!line.Intersects(segment.GetLine())) continue;
                collidedWith = segment.GetOwnerPid();
                return true;
            }

            return false;
        }

        public BikeDto GetDto()
        {
            return new BikeDto
            {
                Pos = new Vector2(_pos.X, _pos.Y),
                Dir = new Vector2(_dir.X, _dir.Y),
                Spd = _spd,
                Trail = _trail.GetList().Select(t => t.GetDto()).ToList(),
                Colour = $"rgba({_colour.R},{_colour.G},{_colour.B},%A%)"
            };
        }

        public Vector2 GetPos()
        {
            return _pos;
        }

        public void SetPos(Vector2 pos)
        {
            this._pos = pos;
        }

        public Point GetPosAsPoint()
        {
            return new Point((int)_pos.X, (int)_pos.Y);
        }

        public Vector2 GetDir()
        {
            return _dir;
        }

        public float GetSpd()
        {
            return _spd;
        }

        public void SetSpd(float spd)
        {
            _spd = spd;
        }

        public void SetDir(Vector2 dir)
        {
            if (_dir.X == 0 && dir.X == 0 || _dir.Y == 0 && dir.Y == 0)
                return;

            _dir = dir;
            AddTrailPoint();
        }

        public List<TrailSegment> GetTrailSegmentList(bool withHead)
        {
            if (!withHead)
                return _trail.GetList();

            var trailWithHead = new List<TrailSegment>(_trail.GetList());
            float headSegmentStartX;
            float headSegmentStartY;

            if (_trail.Size() > 0)
            {
                var head = _trail.GetHead();
                if (head == null)
                {
                    return new List<TrailSegment>();
                }
                var lastSeg = _trail.GetHead().GetLine();
                headSegmentStartX = lastSeg.End.X;
                headSegmentStartY = lastSeg.End.Y;
            }
            else
            {
                headSegmentStartX = _startPos.X;
                headSegmentStartY = _startPos.Y;
            }

            foreach (var t in trailWithHead)
            {
                t.SetHead(false);
            }
            var headSegment = new TrailSegment(_ownerPid, new LineSegment2D(new Vector2(headSegmentStartX, headSegmentStartY), new Vector2(_pos.X, _pos.Y)));
            headSegment.SetHead(true);
            trailWithHead.Add(headSegment);

            return trailWithHead;
        }

        public Color GetColour()
        {
            return _colour;
        }

        public void Crash()
        {
            SetDir(Vector2.Zero);
            AddTrailPoint();
        }

        public override string ToString()
        {
            return "bike s(" + _pos.X + ", " + _pos.Y + "), s(" + _dir.X + ", " + _dir.Y + ")";
        }

        public void BreakTrailSegment(ImpactPoint impactPoint, double radius)
        {
            _trail.BreakSegment(impactPoint, radius);
        }
    }
}
