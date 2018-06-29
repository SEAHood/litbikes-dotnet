using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace LitBikes.Model
{
    public class DebugDto
    {
        public List<ImpactDto> impacts;
    }

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
            DebugDto dto = new DebugDto
            {
                impacts = new List<ImpactDto>()
            };

            foreach (var impact in impacts)
            {
                ImpactDto impactDto = new ImpactDto
                {
                    pos = new Vector2(impact.GetPoint().X, impact.GetPoint().Y)
                };
                dto.impacts.Add(impactDto);
            }
            return dto;
        }
    }
}
