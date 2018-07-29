namespace LitBikes.Model.Dtos.Short
{
    public class ArenaDtoShort : IDtoShort
    {
        public int S { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new ArenaDto
            {
                Size = S
            };
            return dto;
        }
    }
}
