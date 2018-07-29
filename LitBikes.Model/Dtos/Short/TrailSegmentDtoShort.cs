using System.Numerics;

namespace LitBikes.Model.Dtos.Short
{
    public class TrailSegmentDtoShort : IDtoShort
    {
        public bool Ih;
        public Vector2 S;
        public Vector2 E;

        public IDto MapToFullDto()
        {
            var dto = new TrailSegmentDto
            {
                Start = S,
                End = E,
                IsHead = Ih
            };
            return dto;
        }
    }
}
