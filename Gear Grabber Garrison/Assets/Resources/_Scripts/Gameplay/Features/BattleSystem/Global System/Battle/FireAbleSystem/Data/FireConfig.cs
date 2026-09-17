using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewFireConfig", menuName = "GGG/Battle/Fire Config")]
    public class FireConfig : ScriptableObject
    {
        [Header("Интерсивность горения")]
        public StatParam FireStrange = new StatParam{
            type = StatType.FireStrange, 
            value = 10f
        };
        [Header("Продолжительность горения")]
        public StatParam FireTime = new StatParam{
            type = StatType.FireTime, 
            value = 4f
        };
    }
}