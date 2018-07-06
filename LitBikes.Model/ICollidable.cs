using System;

namespace LitBikes.Model
{
    public interface ICollidable
    {
        Guid GetId();
        string GetName();
    }
}
