using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Numerics;
using LitBikes.Model.Dtos;

namespace LitBikes.Model
{
    public enum PowerUpType
    {
        Nothing,
        Rocket,
        Slow
    }

    public class PowerUp
    {
        private readonly Guid _id;
        private Vector2 _pos;
        private PowerUpType _type;
        private bool _collected;

        public DateTime DespawnTime { get; set; }

        public PowerUp(Vector2 pos, PowerUpType type)
        {
            _id = Guid.NewGuid();
            _pos = pos;
            _type = type;
            _collected = false;
        }

        public Guid GetId()
        {
            return _id;
        }

        public Vector2 GetPos()
        {
            return _pos;
        }

        public void SetPos(Vector2 pos)
        {
            this._pos = pos;
        }

        public PowerUpType GetPowerUpType()
        {
            return _type;
        }

        public void SetType(PowerUpType type)
        {
            _type = type;
        }

        public bool IsCollected()
        {
            return _collected;
        }

        public void SetCollected(bool collected)
        {
            _collected = collected;
        }

        public bool Collides(LineSegment2D line)
        {
            const int boxSize = 6;
            var originX = (int)_pos.X - (boxSize / 2);
            var originY = (int)_pos.Y - (boxSize / 2);
            var hitbox = new Rectangle(originX, originY, boxSize, boxSize);
            return line.Intersects(hitbox);
        }

        public bool Equals(PowerUp other)
        {
            if (other == null) return false;
            return _id == other.GetId() &&
                   _pos == other.GetPos() &&
                   _type == other.GetPowerUpType() &&
                   _collected == other._collected;                   
        }

        public PowerUpDto GetDto()
        {
            return new PowerUpDto
            {
                Id = _id.ToString(),
                Pos = _pos,
                Type = _type,
                Collected = _collected
            };
        }
    }
}
