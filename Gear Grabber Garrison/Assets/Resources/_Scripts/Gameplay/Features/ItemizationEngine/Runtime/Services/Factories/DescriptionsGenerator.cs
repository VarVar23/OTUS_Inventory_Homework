using System.Collections.Generic;
using System.Text;
using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal static class DescriptionGenerator
    {
        public static void FillAttributeDescription(ItemAttribute attribute)
        {
            AttributeDataModel model = attribute.DataModel;
            if (model == null || model.RuleChains.Count == 0) return;

            RuleChainModel primaryChain = model.RuleChains[0];

            BuildDescriptionLines(primaryChain, model, attribute, out StringBuilder descriptionStringBuilder, out StringBuilder upgradeStringBuilder);

            attribute.UpdateDescriptions(descriptionStringBuilder.ToString().TrimEnd(), upgradeStringBuilder.ToString().TrimEnd());
        }

        private static void BuildDescriptionLines(RuleChainModel primaryChain, AttributeDataModel model, ItemAttribute attribute, out StringBuilder descriptionStringBuilder, out StringBuilder upgradeStringBuilder)
        {
            descriptionStringBuilder = new StringBuilder();
            upgradeStringBuilder = new StringBuilder();

            RuleStepModel step;
            int currentStepValue, nextStepValue;
            string upgradeString;

            for (int i = 0; i < primaryChain.Steps.Count; i++)
            {
                step = primaryChain.Steps[i];
                if (string.IsNullOrEmpty(step.DescriptionTemplate)) continue;

                bool hasPlaceholder = step.DescriptionTemplate.Contains("{1}");
                if (!hasPlaceholder)
                {
                    descriptionStringBuilder.Append(step.DescriptionTemplate);
                    upgradeStringBuilder.Append(step.DescriptionTemplate);
                    continue;
                }

                currentStepValue = GetValueFromModel(model, attribute.Level, step.ValueKey);
                descriptionStringBuilder.Append(step.DescriptionTemplate.Replace("{1}", currentStepValue.ToString()) + " ");

                if (attribute.CanUpgrade)
                {
                    nextStepValue = GetValueFromModel(model, attribute.Level + 1, step.ValueKey);
                    
                    if (currentStepValue != nextStepValue)
                    {
                        upgradeString = $"{currentStepValue} -> {nextStepValue}";
                        upgradeStringBuilder.Append(step.DescriptionTemplate.Replace("{1}", upgradeString) + " ");
                    }
                    else
                    {
                        upgradeStringBuilder.Append(step.DescriptionTemplate.Replace("{1}", currentStepValue.ToString()) + " ");
                    }
                }
                else
                {
                    upgradeStringBuilder.Append(step.DescriptionTemplate.Replace("{1}", currentStepValue.ToString()) + " ");
                }
            }
        }

        public static void FillItemDescription(Item item)
        {
            if (item.Attributes.Count == 0)
            {
                item.UpdateDescriptions("No attributes.", "No attributes.");
                return;
            }

            StringBuilder mainStringBuilder = new();
            StringBuilder upgradeStringBuilder = new();

            for (int i = 0; i < item.Attributes.Count; i++)
            {
                if (!string.IsNullOrEmpty(item.Attributes[i].Description))
                    mainStringBuilder.AppendLine("• " + item.Attributes[i].Description);
                
                if (!string.IsNullOrEmpty(item.Attributes[i].UpgradeDescription))
                    upgradeStringBuilder.AppendLine("• " + item.Attributes[i].UpgradeDescription);
            }

            item.UpdateDescriptions(mainStringBuilder.ToString().TrimEnd(), upgradeStringBuilder.ToString().TrimEnd());
        }

        public static string GenerateOwnerSummary(List<Item> items)
        {
            if (items == null || items.Count == 0) return "No active effects.";

            int totalCount = 0;
            for (int i = 0; i < items.Count; i++)
                totalCount += items[i].Attributes.Count;

            List<ItemAttribute> allAttributes = new(totalCount);
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = 0; j < items[i].Attributes.Count; j++)
                    allAttributes.Add(items[i].Attributes[j]);
            }

            return GenerateOwnerSummary(allAttributes);
        }

        private static string GenerateOwnerSummary(IReadOnlyList<ItemAttribute> attributes)
        {
            if (attributes == null || attributes.Count == 0) return "No active effects.";

            Dictionary<Stat, (int sum, string template)> statTotals = new();
            Dictionary<string, (int count, string description)> effectTotals = new();

            ItemAttribute itemAttribute;
            RuleChainModel primaryChain;
            RuleStepModel step;
            int attributeValue;

            for (int i = 0; i < attributes.Count; i++)
            {
                itemAttribute = attributes[i];
                if (itemAttribute.DataModel == null || itemAttribute.DataModel.RuleChains.Count == 0) continue;

                primaryChain = itemAttribute.DataModel.RuleChains[0];

                for (int j = 0; j < primaryChain.Steps.Count; j++)
                {
                    step = primaryChain.Steps[j];
                    if (step.Type == StepType.ModifyStat)
                    {
                        attributeValue = GetValueFromModel(itemAttribute.DataModel, itemAttribute.Level, step.ValueKey);
                        if (statTotals.TryGetValue(step.TargetStat, out var data))
                        {
                            statTotals[step.TargetStat] = (data.sum + attributeValue, data.template);
                        }
                        else
                        {
                            statTotals[step.TargetStat] = (attributeValue, step.DescriptionTemplate);
                        }
                    }
                    else
                    {
                        if (effectTotals.TryGetValue(itemAttribute.Id, out var data))
                            effectTotals[itemAttribute.Id] = (data.count + 1, data.description);
                        else
                            effectTotals[itemAttribute.Id] = (1, itemAttribute.Description);
                        break;
                    }
                }
            }

            StringBuilder ownerSummaryStringBuilder = new();

            List<KeyValuePair<Stat, (int sum, string template)>> statEntries = new(statTotals);

            for (int i = 0; i < statEntries.Count; i++)
            {
                if (string.IsNullOrEmpty(statEntries[i].Value.template)) continue;

                ownerSummaryStringBuilder.AppendLine(statEntries[i].Value.template.Replace("{1}", statEntries[i].Value.sum.ToString()));
            }

            List<(int count, string desc)> effectEntries = new(effectTotals.Values);

            for (int i = 0; i < effectEntries.Count; i++)
            {
                if (effectEntries[i].count > 1)
                    ownerSummaryStringBuilder.AppendLine($"{effectEntries[i].count}x {effectEntries[i].desc}");
                else
                    ownerSummaryStringBuilder.AppendLine(effectEntries[i].desc);
            }

            return ownerSummaryStringBuilder.ToString().TrimEnd();
        }

        private static int GetValueFromModel(AttributeDataModel model, int level, string key)
        {
            if (string.IsNullOrEmpty(key)) return 0;

            AttributeLevelData levelData;
            AttributeValueEntry valueEntry;

            for (int i = 0; i < model.Levels.Count; i++)
            {
                levelData = model.Levels[i];
                if (levelData.Level != level) continue;

                for (int j = 0; j < levelData.Values.Count; j++)
                {
                    valueEntry = levelData.Values[j];
                    if (valueEntry.Key == key) return valueEntry.Value;
                }
            }
            return 0;
        }
    }
}