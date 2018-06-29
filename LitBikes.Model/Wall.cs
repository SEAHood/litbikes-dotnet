using System;

namespace LitBikes.Model
{
    public class Wall : ICollidable
    {
        public int GetId()
        {
            return -1;
        }
        public String GetName()
        {
            return "The Wall";
        }
    }
}
