namespace LitBikes.Model
{
    public class ArenaDto
    {
        public int size;
    }

    public class Arena
    {
        public int size;

        public Arena(int gameSize)
        {
            size = gameSize;
        }

        public ArenaDto GetDto()
        {
            return new ArenaDto
            {
                size = size
            };
        }

        public bool CheckCollision(Bike bike, int lookAhead)
        {
            var bPos = bike.GetPos();
            double collisionX = bPos.X + (lookAhead * bike.GetDir().X);
            double collisionY = bPos.Y + (lookAhead * bike.GetDir().Y);
            return collisionX >= size || collisionX <= 0 || collisionY >= size || collisionY <= 0;
        }
    }
}
