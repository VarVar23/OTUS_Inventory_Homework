using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal static class ResolutionPayloadMapper
    {
        public static IReadOnlyList<IResolutionPayload> Map(IReadOnlyList<ResolutionContext> contexts)
        {
            var output = new List<IResolutionPayload>(contexts.Count);

            for (int i = 0; i < contexts.Count; i++)
            {
                if (contexts[i] is EffectResolutionContext effect)
                {
                    output.Add(new EffectResolutionPayload
                    (
                        effect.ItemOwnerId,
                        effect.TargetId,
                        effect.Effect,
                        effect.Value
                    ));
                    continue;
                }

                if (contexts[i] is StatResolutionContext stat)
                {
                    output.Add(new StatResolutionPayload
                    (
                        stat.ItemOwnerId,
                        stat.TargetId,
                        stat.Stat,
                        stat.ModificationType,
                        stat.Value
                    ));
                }
            }

            return output;
        }
    }
}