using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using LitBikes.Model.Dtos;

namespace LitBikes.Model
{
    public class Debug
    {
        private List<ImpactPoint> impacts;

        public Debug()
        {
            impacts = new List<ImpactPoint>();
        }

        public void AddImpact(ImpactPoint ip)
        {
            impacts.Add(ip);
            var ignoredAwait = WaitAndRemoveImpact(ip);
        }

        private async Task WaitAndRemoveImpact(ImpactPoint ip)
        {
            await Task.Delay(100);
            impacts.Remove(ip);
        }

        public DebugDto GetDto()
        {
            var dto = new DebugDto
            {
                Impacts = new List<ImpactDto>()
            };

            foreach (var impact in impacts)
            {
                var impactDto = new ImpactDto
                {
                    Pos = new Vector2(impact.GetPoint().X, impact.GetPoint().Y)
                };
                dto.Impacts.Add(impactDto);
            }
            return dto;
        }
    }
}
