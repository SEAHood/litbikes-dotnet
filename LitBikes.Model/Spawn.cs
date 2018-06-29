using LitBikes.Util;
using System.Numerics;

namespace LitBikes.Model
{
    public class Spawn
    {

        private readonly Vector2 pos;
	    private readonly Vector2 dir;
	    private readonly float spd;

        public Spawn(int gameSize)
        {
            pos = CreateSpawnPosition(gameSize);
            dir = CreateSpawnDir();
            spd = GameEngine.BASE_BIKE_SPEED;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public Vector2 GetDir()
        {
            return dir;
        }

        public float GetSpd()
        {
            return spd;
        }

        private Vector2 CreateSpawnPosition(int gameSize)
        {
            return new Vector2(NumberUtil.randInt(20, gameSize - 20), NumberUtil.randInt(20, gameSize - 20));
        }

        private Vector2 CreateSpawnDir()
        {
            int dir = NumberUtil.randInt(1, 4);
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
