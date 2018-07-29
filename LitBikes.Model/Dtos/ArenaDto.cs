using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class ArenaDto : IDto
    {
        public int Size { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ArenaDtoShort
            {
                S = Size
            };
            return shortDto;
        }
    }
}
