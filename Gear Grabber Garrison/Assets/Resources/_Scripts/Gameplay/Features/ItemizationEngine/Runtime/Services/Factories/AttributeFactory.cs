using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal static class AttributeFactory
    {
        public static ItemAttribute Build(AttributeDataModel model, int level)
        {
            var itemAttr = new ItemAttribute(model.AttributeId);
            RuleChainModel chainData;
            RuleStepModel step;

            for (int i = 0; i < model.RuleChains.Count; i++)
            {
                chainData = model.RuleChains[i];
                RuleChain liveChain = new(chainData.Trigger);
                
                for (int j = 0; j < chainData.Steps.Count; j++)
                {
                    step = chainData.Steps[j];
                    switch (step.Type)
                    {
                        case StepType.ModifyStat:
                            liveChain.ChangeStat(step.TargetStat, step.Mod)
                                    .WithValues(step.ValueKey);
                            break;

                        case StepType.RunEffect:
                            liveChain.Do(step.Effect);
                            break;

                        case StepType.RunEffectWithValue:
                            liveChain.Do(step.Effect)
                                    .WithValues(step.ValueKey);
                            break;

                        case StepType.ChanceGate:
                            liveChain.Where(ctx => {
                                float chance = ctx.Get(step.ValueKey);
                                return UnityEngine.Random.Range(0f, 100f) <= chance;
                            });
                            break;
                    }
                }
                
                itemAttr.AddChain(liveChain);
            }

            itemAttr.Initialize(model, level);

            DescriptionGenerator.FillAttributeDescription(itemAttr);

            return itemAttr;
        }
    }
}