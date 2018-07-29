using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public class PowerUpDtoShort : IDto
    {
        public string I { get; set; }
        public string N { get; set; }
        public Vector2 P { get; set; }
        public PowerUpType T { get; set; }
        public bool C { get; set; }
    }
}
