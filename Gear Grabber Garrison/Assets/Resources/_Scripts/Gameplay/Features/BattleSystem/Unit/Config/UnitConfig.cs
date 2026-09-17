using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "GGG/Battle/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public string Name = "New Unit";
        public GameObject Prefab;

        public float RespawnDelay = 60f;
        
        public Vector3 SuprimTarget ;
        
        public MoveConfig MoveConfig;
        //public HealthConfig HealthConfig;
        
        
    }
}