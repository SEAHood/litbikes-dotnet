using System;
using System.Numerics;
using LitBikes.Model.Dtos.FromClient.Short;

namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientUpdateDto : IDto
    {
        public Guid PlayerId { get; set; } //TODO probably shouldn't rely on this!!!!!!!
        public int? XDir { get; set; }
        public int? YDir { get; set; }
        public int? XPos { get; set; }
        public int? YPos { get; set; }

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

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ClientUpdateDtoShort
            {
                I = PlayerId,
                Xd = XDir,
                Yd = YDir,
                Xp = XPos,
                Yp = YPos
            };
            return shortDto;
        }
    }
}
