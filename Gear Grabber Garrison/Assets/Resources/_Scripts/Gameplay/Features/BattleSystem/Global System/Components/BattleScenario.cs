using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "New_BattleScenario", menuName = "GGG/Battle/Scenario")]
    public class BattleScenario : ScriptableObject
    {
        public ArmyConfig PlayerArmy;
        public WaveConfig Waves;
        
        public float IntervalBetweenWaves = 5f;
    }
}