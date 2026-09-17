using System;
using System.Collections.Generic;

namespace Gameplay.Itemization.Models
{
    [Serializable]
    internal sealed class ItemBlueprintModel
    {
        public string BlueprintId;
        public int AttributeLevelBudget = 1;
        public bool ForceFullBudget = true;
        public List<TypeWeightModel> TypeWeights = new();
        public List<AttributeSlotModel> Slots = new();
    }

    [Serializable]
    internal sealed class TypeWeightModel
    {
        public ItemType Type;
        public float Weight = 1.0f;
    }

    [Serializable]
    internal sealed class AttributeSlotModel
    {
        public string SlotName = "New Slot";
        public int MinAllowedLevel = 1;
        public int MaxAllowedLevel = 1;
        public List<AttributeWeightModel> PossibleAttributes = new();
    }

    [Serializable]
    internal sealed class AttributeWeightModel
    {
        public string AttributeId;
        public float Weight = 1.0f;
    }
}