using System.Collections.Generic;
using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public class BikeDto
    {
        public Vector2 Pos;
        public Vector2 Dir;
        public float Spd;
        public List<TrailSegmentDto> Trail;
        public string Colour; // in rgba(0,0,0,%A%) format
    }
}
