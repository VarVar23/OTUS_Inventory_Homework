using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewUnitComposition", menuName = "GGG/Battle/Unit Composition")]
    public class UnitComposition : ScriptableObject
    {
        [Header("Core Systems")]
        public HealthConfig HealthConfig;
        public MoveConfig MoveConfig;
        public AttackConfig AttackConfig;
        public StunConfig StunConfig;
        
        [Header("Dynamic Systems (can be added runtime)")]
        public List<SystemType> AvailableDynamicSystems = new List<SystemType>();
        
        [Header("Fire System")]
        public FireConfig FireConfig;
        
        public bool HasSystem(SystemType type)
        {
            return type switch
            {
                SystemType.HealthSystem => HealthConfig != null,
                SystemType.MoveSystem => MoveConfig != null,
                SystemType.AttackSystem => AttackConfig != null,
                SystemType.StunSystem => StunConfig != null,
                SystemType.FireSystem => FireConfig != null,
                _ => false
            };
        }
    }
}