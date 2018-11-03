using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public class TrailSegmentDto : IDto
    {
        public bool IsHead;
        public Vector2 Start;
        public Vector2 End;
    }
}
