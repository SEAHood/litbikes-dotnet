using LitBikes.Util;
using System.Numerics;

namespace LitBikes.Model
{
    public class Spawn
    {
        private readonly Vector2 _pos;
	    private readonly Vector2 _dir;
	    private readonly float _spd;

        public Spawn(int gameSize, float speed)
        {
            _pos = CreateSpawnPosition(gameSize);
            _dir = CreateSpawnDir();
            _spd = speed;
        }

        public Vector2 GetPos()
        {
            return _pos;
        }

        public Vector2 GetDir()
        {
            return _dir;
        }

        public float GetSpd()
        {
            return _spd;
        }

        private static Vector2 CreateSpawnPosition(int gameSize)
        {
            return new Vector2(NumberUtil.RandInt(20, gameSize - 20), NumberUtil.RandInt(20, gameSize - 20));
        }

        private static Vector2 CreateSpawnDir()
        {
            var dir = NumberUtil.RandInt(1, 4);
            switch (dir)
            {
                case 1:
                    return new Vector2(0, -1);
                case 2:
                    return new Vector2(0, 1);
                case 3:
                    return new Vector2(-1, 0);
                case 4:
                    return new Vector2(1, 0);
                default:
                    return Vector2.Zero; // Won't happen
            }
        }

    }
}
