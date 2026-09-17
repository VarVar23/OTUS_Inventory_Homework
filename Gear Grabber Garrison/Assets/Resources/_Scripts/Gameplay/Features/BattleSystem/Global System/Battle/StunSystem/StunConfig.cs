using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewStunConfig", menuName = "GGG/Battle/Unit/StunConfig")]
    public class StunConfig : ScriptableObject
    {
        [Header("Stun Resistance")]
        [Range(0f, 1f)] public float StunAvoidance = 0f;
        [Range(0f, 1f)] public float StunResistance = 0f;
    }
}