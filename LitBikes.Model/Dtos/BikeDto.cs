using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class BikeDto : IDto
    {
        public Vector2 Pos;
        public Vector2 Dir;
        public float Spd;
        public List<TrailSegmentDto> Trail;
        public string Colour; // in rgba(0,0,0,%A%) format

        public IDtoShort MapToShortDto()
        {
            var shortDto = new BikeDtoShort
            {
                D = Dir,
                C = Colour,
                P = Pos,
                S = Spd,
                T = Trail.Select(t => (TrailSegmentDtoShort) t.MapToShortDto()).ToList()
            };
            return shortDto;
        }
    }
}
