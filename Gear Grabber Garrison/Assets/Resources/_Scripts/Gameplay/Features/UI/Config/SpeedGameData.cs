using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.UI
{
    [CreateAssetMenu(fileName = "SpeedGameData", menuName = "Config/SpeedGameData")]
    internal class SpeedGameData : ScriptableObject
    {
       [field: SerializeField] public List<int> Speed { get; private set; }
    }
}