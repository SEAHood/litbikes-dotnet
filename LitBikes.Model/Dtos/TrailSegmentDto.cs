using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class TrailSegmentDto : IDto
    {
        public bool IsHead;
        public Vector2 Start;
        public Vector2 End;

        public IDtoShort MapToShortDto()
        {
            var shortDto = new TrailSegmentDtoShort
            {
                S = Start,
                E = End,
                Ih = IsHead
            };
            return shortDto;
        }
    }
}
