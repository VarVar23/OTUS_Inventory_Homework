using System;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    [Serializable]
    public class UnitSpawnData
    {
        public GameObject SpawnPoint;   // null = использовать армейский
        public GameObject TargetPoint;  // null = SpawnPoint юнита или армейский
    }
}