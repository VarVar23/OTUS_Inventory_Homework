using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewAttackConfig", menuName = "GGG/Battle/Attack Config")]
    public class AttackConfig : ScriptableObject
    {
        [Header("Характеристики Атаки")]
        public AttackType Type;
        public int MinDamage = 20;
        public int MaxDamage = 25;
        public float AttackRange;
        public float AttackSpeed = 1;
        public float Cooldown;
        public float Crit;
        
        [Header("Animation")]
        public float AttackAnimationDuration = 0.5f; // Длительность анимации атаки
        
        [Header("Stun")]
        [Range(0f, 1f)] public float StunChance = 0f;
        public float StunDuration = 2f;
    }
}
