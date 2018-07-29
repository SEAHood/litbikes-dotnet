using System;

namespace LitBikes.Model.Dtos.FromClient.Short
{
    public class ClientUpdateDtoShort : IDtoShort
    {
        public Guid I; //TODO probably shouldn't rely on this!!!!!!!
        public int? Xd;
        public int? Yd;
        public int? Xp;
        public int? Yp;

        public IDto MapToFullDto()
        {
            var dto = new ClientUpdateDto
            {
                PlayerId = I,
                XDir = Xd,
                YDir = Yd,
                XPos = Xp,
                YPos = Yp
            };
            return dto;
        }
    }
}
