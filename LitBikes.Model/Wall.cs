using System;

namespace LitBikes.Model
{
    public class Wall : ICollidable
    {
        public Guid GetId()
        {
            return Guid.Empty;
        }
        public string GetName()
        {
            return "The Wall";
        }
    }
}
