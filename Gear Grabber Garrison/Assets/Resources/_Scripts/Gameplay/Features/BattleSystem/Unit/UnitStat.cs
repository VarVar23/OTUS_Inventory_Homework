using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class UnitStat : MonoBehaviour //, IHasStatTable
    {
        public Guid Id { get; private set; }
        // STAT система
        private readonly Dictionary<StatType, float> _stats = new();
        
        public float GetStat(StatType statType) => _stats.GetValueOrDefault(statType, 0f);
        
        public void SetStat(StatType statType, float value)
        {
            float oldValue = _stats.GetValueOrDefault(statType, 0f);
            _stats[statType] = value;
            // OnStatChanged?.Invoke(stat, oldValue, value);
        }
    }
}