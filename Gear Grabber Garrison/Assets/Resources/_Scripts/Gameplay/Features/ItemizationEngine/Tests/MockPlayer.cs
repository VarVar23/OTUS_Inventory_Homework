using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.Itemization.Tests 
{
    public class MockPlayer : MonoBehaviour
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        private readonly Dictionary<Stat, float> _stats = new() { { Stat.MaxHealth, 100f }};
        private readonly Dictionary<ItemType, IItem> _equippedSlots = new();

        public float GetStat(Stat stat) => _stats.GetValueOrDefault(stat, 0f);
        public void SetStat(Stat stat, float value) 
        {
            float oldValue = _stats.GetValueOrDefault(stat, 0f);
            Debug.Log($"<color=orange>[MockPlayer]</color> Stat {stat} changed from {oldValue} to {value}");
            _stats[stat] = value;
        }

        public bool TryEquip(IItem item)
        {
            if (_equippedSlots.ContainsKey(item.Type))
            {
                Debug.LogWarning($"<color=orange>[MockPlayer]</color> Cannot equip {item.ItemName}. Slot {item.Type} is already occupied by {_equippedSlots[item.Type].ItemName}.");
                return false;
            }

            _equippedSlots[item.Type] = item;
            return true;
        }

        public bool TryUnequipAny(out IItem item)
        {
            if (_equippedSlots.Count == 0)
            {
                item = null;
                Debug.LogWarning("<color=orange>[MockPlayer]</color> No items equipped to unequip.");
                return false;
            }

            // Grab the first available item (e.g., Weapon, then Chest, etc.)
            var kvp = _equippedSlots.First();
            item = kvp.Value;
            _equippedSlots.Remove(kvp.Key);
            return true;
        }

        public Action<float> GetEffectWithValue(AttributeEffect effect)
        {
            return effect switch {
                AttributeEffect.DisplayMessageWithValue => (value) =>
                {
                    if (Mathf.Approximately(value, 1f))
                    {
                        Debug.Log("<color=green>[MockPlayer] Item equipped.</color>");
                        return;
                    }

                    if (Mathf.Approximately(value, 0f))
                    {
                        Debug.Log("<color=yellow>[MockPlayer] Item unequipped.</color>");
                    }
                },
                _ => null
            };
        }
    }
}