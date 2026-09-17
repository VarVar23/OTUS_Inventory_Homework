using System.Collections.Generic;
using Gameplay.Itemization.Models;
using UnityEngine;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class BuildInstructionsGenerator
    {
        private readonly JSONItemizationRegistry _registry;
        private readonly ItemNameGenerator _nameGenerator;
        private readonly ItemIconPicker _iconPicker;

        [Inject]
        public BuildInstructionsGenerator(JSONItemizationRegistry registry, ItemNameGenerator nameGenerator, ItemIconPicker iconPicker)
        {
            _registry = registry;
            _nameGenerator = nameGenerator;
            _iconPicker = iconPicker;
        }

        private struct SlotState
        {
            public string AttributeId;
            public int CurrentLevel;
            public int Min;
            public int Max;
        }

        public BuildInstructions CreateInstructions(ItemBlueprintModel blueprint)
        {
            ValidateBudget(blueprint);

            BuildInstructions instructions = new();

            List<SlotState> slotStates = BuildSlotStates(blueprint, out int currentSpentBudget);

            if (blueprint.ForceFullBudget)
            {
                ReconcileBudget(slotStates, blueprint.AttributeLevelBudget, currentSpentBudget);
            }

            instructions.SelectedType = WeightedRandomService.PickFromList(blueprint.TypeWeights, tw => tw.Weight).Type;

            for (int i = 0; i < slotStates.Count; i++)
            {
                instructions.AttributeMap[slotStates[i].AttributeId] = slotStates[i].CurrentLevel;
            }

            List<string> rolledIds = new(slotStates.Count);
            for (int i = 0; i < slotStates.Count; i++)
            {
                rolledIds.Add(slotStates[i].AttributeId);
            }
            instructions.GeneratedName = _nameGenerator.GenerateName(instructions.SelectedType, rolledIds, out string baseNameUsed);
            instructions.Icon = _iconPicker.GetIcon(baseNameUsed);

            return instructions;
        }

        private List<SlotState> BuildSlotStates(ItemBlueprintModel blueprint, out int currentSpentBudget)
        {
            List<SlotState> slotStates = new();
            currentSpentBudget = 0;

            HashSet<string> rolledAttributesInItem = new();
            AttributeSlotModel slot;
            List<AttributeWeightModel> uniquePossibleAttributes;
            AttributeWeightModel rolledWeightModel;
            string rolledId;
            AttributeDataModel attributeData;
            int attributeMaxLevel, absoluteMax, initialLevel;

            for (int i = 0; i < blueprint.Slots.Count; i++)
            {
                slot = blueprint.Slots[i];
                uniquePossibleAttributes = GetUniquePossibleAttributes(slot, rolledAttributesInItem);

                if (uniquePossibleAttributes.Count == 0)
                    continue;

                rolledWeightModel = WeightedRandomService.PickFromList(uniquePossibleAttributes, aw => aw.Weight);
                rolledId = rolledWeightModel.AttributeId;

                if (string.IsNullOrEmpty(rolledId))
                    continue;

                rolledAttributesInItem.Add(rolledId);
                attributeData = _registry.GetAttribute(rolledId);

                if (attributeData == null)
                {
                    Debug.LogError($"[Generator] Attribute '{rolledId}' not found in Registry!");
                    continue;
                }

                attributeMaxLevel = GetMaxAttributeLevel(attributeData);
                absoluteMax = Mathf.Min(slot.MaxAllowedLevel, attributeMaxLevel);
                initialLevel = Random.Range(slot.MinAllowedLevel, absoluteMax + 1);

                slotStates.Add(new SlotState
                {
                    AttributeId = rolledId,
                    CurrentLevel = initialLevel,
                    Min = slot.MinAllowedLevel,
                    Max = absoluteMax
                });

                currentSpentBudget += initialLevel;
            }

            return slotStates;
        }

        private static List<AttributeWeightModel> GetUniquePossibleAttributes(AttributeSlotModel slot, HashSet<string> rolledAttributesInItem)
        {
            List<AttributeWeightModel> uniquePossibleAttributes = new();
            AttributeWeightModel candidate;
            for (int i = 0; i < slot.PossibleAttributes.Count; i++)
            {
                candidate = slot.PossibleAttributes[i];
                if (!rolledAttributesInItem.Contains(candidate.AttributeId))
                    uniquePossibleAttributes.Add(candidate);
            }

            return uniquePossibleAttributes;
        }

        private static int GetMaxAttributeLevel(AttributeDataModel attributeData)
        {
            if (attributeData.Levels.Count == 0)
                return 1;

            int maxLevel = attributeData.Levels[0].Level;
            for (int i = 1; i < attributeData.Levels.Count; i++)
            {
                if (attributeData.Levels[i].Level > maxLevel)
                    maxLevel = attributeData.Levels[i].Level;
            }

            return maxLevel;
        }

        private void ReconcileBudget(List<SlotState> states, int target, int current)
        {
            int iterations = 0;
            const int MAX_ITERATIONS = 500;

            List<int> eligibleIndices = new(states.Count);

            while (current != target && iterations < MAX_ITERATIONS)
            {
                iterations++;
                bool needsIncrease = current < target;

                eligibleIndices.Clear();
                for (int i = 0; i < states.Count; i++)
                {
                    if (needsIncrease ? states[i].CurrentLevel < states[i].Max : states[i].CurrentLevel > states[i].Min)
                        eligibleIndices.Add(i);
                }

                if (eligibleIndices.Count == 0) break;

                int chosenIndex = eligibleIndices[Random.Range(0, eligibleIndices.Count)];
                SlotState slot = states[chosenIndex];

                if (needsIncrease) { slot.CurrentLevel++; current++; }
                else { slot.CurrentLevel--; current--; }

                states[chosenIndex] = slot;
            }
        }

        private void ValidateBudget(ItemBlueprintModel blueprint)
        {
            int minSum = 0;
            int maxSum = 0;
            for (int i = 0; i < blueprint.Slots.Count; i++)
            {
                minSum += blueprint.Slots[i].MinAllowedLevel;
                maxSum += blueprint.Slots[i].MaxAllowedLevel;
            }

            if (blueprint.AttributeLevelBudget < minSum || (blueprint.ForceFullBudget && blueprint.AttributeLevelBudget > maxSum))
            {
                Debug.LogWarning($"[Generator] Budget {blueprint.AttributeLevelBudget} might be impossible for {blueprint.BlueprintId}. " +
                                $"Min: {minSum}, Max: {maxSum}. Proceeding with best-effort.");
            }
        }
    }
}