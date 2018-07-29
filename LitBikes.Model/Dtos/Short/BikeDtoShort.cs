using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace LitBikes.Model.Dtos.Short
{
    public class BikeDtoShort : IDtoShort
    {
        public Vector2 P;
        public Vector2 D;
        public float S;
        public List<TrailSegmentDtoShort> T;
        public string C; // in rgba(0,0,0,%A%) format

        public IDto MapToFullDto()
        {
            var dto = new BikeDto
            {
                Dir = D,
                Colour = C,
                Pos = P,
                Spd = S,
                Trail = T.Select(t => (TrailSegmentDto) t.MapToFullDto()).ToList()
            };
            return dto;
        }
    }
}
