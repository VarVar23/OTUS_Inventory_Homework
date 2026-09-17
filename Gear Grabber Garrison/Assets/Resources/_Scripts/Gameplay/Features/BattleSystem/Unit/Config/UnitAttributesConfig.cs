using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewUnitAttributes", menuName = "GGG/Unit Attributes")]
    public class UnitAttributesConfig : ScriptableObject
    {
        [Header("Movement")] 
        public List<StatParam> Move = new List<StatParam>
        {
            new StatParam { type = StatType.MoveSpeed, value = 1f },
            new StatParam { type = StatType.MovePatrolRadius, value = 8f }
        };

        [Header("Combat")]
        public List<StatParam> Combat = new List<StatParam>
        {
            new StatParam { type = StatType.AttackClass, value = 0f },
            new StatParam { type = StatType.AttackRange, value = 1f },
            new StatParam { type = StatType.AttackMaxDamage, value = 25f },
            new StatParam { type = StatType.AttackSpeed, value = 1.2f }
        };

        [Header("Health")]
        public List<StatParam> Health = new List<StatParam>
        {
            new StatParam { type = StatType.HealthMax, value = 100f },
            new StatParam { type = StatType.HealthRegen, value = 1f },
            new StatParam { type = StatType.HealthRegenRate, value = 10f },
        };

        [Header("Stun")]
        public List<StatParam> Stun = new List<StatParam>
        {
            new StatParam { type = StatType.StunResistance, value = 0f }
        };

        public SystemAttributes BuildAttributes()
        {
            var attrs = new SystemAttributes();

            foreach (var attr in Move) attrs.Add(Copy(attr));
            foreach (var attr in Combat) attrs.Add(Copy(attr));
            foreach (var attr in Health) attrs.Add(Copy(attr));
            foreach (var attr in Stun) attrs.Add(Copy(attr));

            return attrs;
        }

        private StatParam Copy(StatParam original) =>
            new StatParam { type = original.type, value = original.value };
    }
}