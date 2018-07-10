using LitBikes.Model.Dtos;

namespace LitBikes.Model
{
    public class Arena
    {
        public int Size;

        public Arena(int gameSize)
        {
            Size = gameSize;
        }

        public ArenaDto GetDto()
        {
            return new ArenaDto
            {
                Size = Size
            };
        }

        public bool CheckCollision(Bike bike, int lookAhead)
        {
            var bPos = bike.GetPos();
            double collisionX = bPos.X + (lookAhead * bike.GetDir().X);
            double collisionY = bPos.Y + (lookAhead * bike.GetDir().Y);
            return collisionX >= Size || collisionX <= 0 || collisionY >= Size || collisionY <= 0;
        }
    }
}
