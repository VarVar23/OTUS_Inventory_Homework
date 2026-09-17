using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "ArmyConfig", menuName = "Battle/Army Config")]
    public class ArmyConfig : ScriptableObject
    {
        [Header("Unit")]
        public GameObject UnitPrefab;
        public int UnitCount;
        public UnitAttributesConfig UnitAttributesConfig;

        [Header("Spawn")]
        public GameObject SpawnPoint;
        public GameObject TargetPoint;
        public SpawnMode SpawnMode = SpawnMode.Immediate;
        public float SpawnInterval = 1f;
        public float SpawnSpreadX = 1f;

        [Header("Respawn")]
        public bool CanRespawn = false;
        public float RespawnDelay = 5f;

        [Header("Per-Unit Overrides")]
        public List<UnitSpawnData> UnitOverrides;

        [Header("Completion")]
        public float SurviveTime = 0f;  // 0 = только AllDead
    }
}