using System;
namespace LitBikes.Model.Dtos
{
    public class ScoreDtoShort : IDto
    {
        public Guid I { get; set; }
        public string N { get; set; }
        public int S { get; set; }
    }
}
