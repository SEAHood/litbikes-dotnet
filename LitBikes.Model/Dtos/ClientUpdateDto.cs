using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public class ClientUpdateDto
    {
        public int? pid;
        public int? xDir;
        public int? yDir;
        public int? xPos;
        public int? yPos;

        public Vector2? GetDir()
        {
            if (xDir != null && yDir != null)
                return new Vector2(xDir.Value, yDir.Value);
            else
                return null;
        }

        public bool IsValid()
        {
            return pid != null &&
                   xDir != null && xDir <= 1 && xDir >= -1 &&
                   yDir != null && yDir <= 1 && yDir >= -1;
        }
    }
}
