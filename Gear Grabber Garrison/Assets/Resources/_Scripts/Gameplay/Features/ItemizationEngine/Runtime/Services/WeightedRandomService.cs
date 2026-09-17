using System.Collections.Generic;
using Gameplay.Itemization.Models;
using UnityEngine;

namespace Gameplay.Itemization.Services
{
    internal static class WeightedRandomService
    {
        public static bool TryPick(DropTableModel table, out string blueprintId)
        {
            float itemWeight = 0f;
            for (int i = 0; i < table.PotentialDrops.Count; i++)
                itemWeight += table.PotentialDrops[i].Weight;
            float totalWeight = itemWeight + table.NoItemDropWeight;

            if (totalWeight <= 0)
            {
                blueprintId = null;
                return false;
            }

            float roll = Random.Range(0, totalWeight);

            if (roll < table.NoItemDropWeight)
            {
                blueprintId = null;
                return false;
            }

            float cursor = table.NoItemDropWeight;
            for (int i = 0; i < table.PotentialDrops.Count; i++)
            {
                cursor += table.PotentialDrops[i].Weight;
                if (roll <= cursor)
                {
                    blueprintId = table.PotentialDrops[i].BlueprintId;
                    return true;
                }
            }

            blueprintId = table.PotentialDrops[table.PotentialDrops.Count - 1].BlueprintId;
            return true;
        }

        public static T PickFromList<T>(List<T> list, System.Func<T, float> weightSelector)
        {
            float totalWeight = 0f;
            for (int i = 0; i < list.Count; i++)
                totalWeight += weightSelector(list[i]);
            if (totalWeight <= 0) return default;

            float roll = Random.Range(0, totalWeight);
            float cursor = 0;

            for (int i = 0; i < list.Count; i++)
            {
                cursor += weightSelector(list[i]);
                if (roll <= cursor) return list[i];
            }

            return list[list.Count - 1];
        }
    }
}