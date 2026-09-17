using System;
using System.Collections.Generic;

namespace Gameplay.Itemization.Models
{
    [Serializable]
    internal sealed class DropTableModel
    {
        public string TableId;
        public float NoItemDropWeight;
        public List<BlueprintWeightModel> PotentialDrops = new();
    }

    [Serializable]
    internal sealed class BlueprintWeightModel
    {
        public string BlueprintId;
        public float Weight = 1.0f;
    }
}