using System;
using System.Numerics;

namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientUpdateDto : IDto
    {
        public Guid PlayerId; //TODO probably shouldn't rely on this!!!!!!!
        public int? XDir;
        public int? YDir;
        public int? XPos;
        public int? YPos;

        public Vector2? GetDir()
        {
            if (XDir != null && YDir != null)
                return new Vector2(XDir.Value, YDir.Value);
            return null;
        }

        public bool IsValid()
        {
            return PlayerId != Guid.Empty &&
                   XDir != null && XDir <= 1 && XDir >= -1 &&
                   YDir != null && YDir <= 1 && YDir >= -1;
        }
    }
}
