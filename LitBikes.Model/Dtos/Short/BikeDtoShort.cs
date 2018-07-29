using System.Collections.Generic;
using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public class BikeDtoShort
    {
        public Vector2 P;
        public Vector2 D;
        public float S;
        public List<TrailSegmentDtoShort> T;
        public string C; // in rgba(0,0,0,%A%) format
    }
}
