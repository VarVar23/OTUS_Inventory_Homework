using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Battle/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        public List<ArmyConfig> Armies;

    }
}