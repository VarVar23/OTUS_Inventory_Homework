using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewMoveConfig", menuName = "GGG/Battle/Move Config")]
    public class MoveConfig : ScriptableObject
    {
        [Header("Скорость перемещение")]
        public StatParam MovementSpeed = new StatParam{
            type = StatType.MoveSpeed, 
            value = 1f
        };
        [Header("радиус выбора новой цели")]
        public StatParam PatrolRadius = new StatParam{
            type = StatType.MovePatrolRadius, 
            value = 10f
        };
    }
}