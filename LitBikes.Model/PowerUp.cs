using LitBikes.Util;
using Nine.Geometry;
using System;
using System.Numerics;

namespace LitBikes.Model
{
    public class PowerUpDto
    {
        public string id;
        public string name;
        public Vector2 pos;
        public PowerUpType type;
        public bool collected;
    }

    public enum PowerUpType
    {
        NOTHING,
        ROCKET,
        SLOW
    }

    public class PowerUp
    {
        private readonly string id;
        private Vector2 pos;
        private PowerUpType type;
        private bool collected;

        public PowerUp(Vector2 _pos, PowerUpType _type)
        {
            id = Guid.NewGuid().ToString();
            pos = _pos;
            type = _type;
            collected = false;
        }

        public string GetId()
        {
            return id;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void SetPos(Vector2 pos)
        {
            this.pos = pos;
        }

        public PowerUpType GetPowerUpType()
        {
            return type;
        }

        public void SetType(PowerUpType type)
        {
            this.type = type;
        }

        public bool IsCollected()
        {
            return collected;
        }

        public void SetCollected(bool collected)
        {
            this.collected = collected;
        }

        public bool Collides(LineSegment2D line)
        {
            int boxSize = 6;
            int originX = (int)pos.X - (boxSize / 2);
            int originY = (int)pos.Y - (boxSize / 2);
            Rectangle hitbox = new Rectangle(originX, originY, boxSize, boxSize);
            return line.Intersects(hitbox);
        }

        public bool Equals(PowerUp other)
        {
            if (other == null) return false;
            return id == other.GetId() &&
                   pos == other.GetPos() &&
                   type == other.GetPowerUpType() &&
                   collected == other.collected;                   
        }

        public PowerUpDto GetDto()
        {
            return new PowerUpDto
            {
                id = id,
                pos = pos,
                type = type,
                collected = collected
            };
        }
    }
}
