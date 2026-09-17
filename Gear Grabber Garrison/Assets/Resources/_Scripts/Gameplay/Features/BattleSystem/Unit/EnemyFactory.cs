using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Gameplay.Features.Battlesystem
{
    public class EnemyFactory : IUnitFactory
    {
        public event Action<Unit> OnUnitDeath;
        // public event Action<Unit> OnUnitDeath
        // {
        //     add { Unit.OnUnitDeath += value; }
        //     remove { Unit.OnUnitDeath -= value; }
        // }
        public Unit Create(GameObject prefab, Vector3 position,
                        ISystemableEntity target, Army army)
        {
            var obj = Object.Instantiate(prefab, position, Quaternion.identity);
            var unit = obj.GetComponent<Unit>();
            if (unit == null)
            {
                Debug.LogError("[EnemyFactory] Prefab has no Unit component!");
                Object.Destroy(obj);
                return null;
            }
            unit.UnitTeam = Team.Enemy;
            unit.Position = position;
            unit.Init();
            unit.SetSupremeTarget(target);
            unit.OnUnitDeath += UnitDeth;
            return unit;
        }

        public void UnitDeth(Unit unit)
        {
            //unit.OnUnitDeath -= UnitDeth;
            OnUnitDeath?.Invoke(unit);
        }
    }
}