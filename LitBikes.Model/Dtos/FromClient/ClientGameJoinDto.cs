using System;

namespace LitBikes.Model.Dtos
{
    public class ClientGameJoinDto : IDto
    {
        public string name;

        public bool IsValid()
        {
            return name.Length > 1 || name.Length <= 15;
        }
    }
}
