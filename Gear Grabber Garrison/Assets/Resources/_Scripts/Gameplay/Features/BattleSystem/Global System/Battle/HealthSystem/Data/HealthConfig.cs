using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewHealthConfig", menuName = "GGG/Battle/Health Config")]
    public class HealthConfig : ScriptableObject
    {
        [Header("Здоровье")]
        public int MaxHealth = 100;
        
        [Header("Регенерация")]
        public int RegenerationValue = 0;
        public float RegenerationSpeed = 1f;
    }
}