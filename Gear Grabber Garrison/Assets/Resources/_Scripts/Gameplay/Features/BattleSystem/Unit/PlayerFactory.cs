using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.Features.Battlesystem
{
    public class PlayerFactory : IUnitFactory
    {
        // [Inject] private TickManager _tickManager;
        public event Action<Unit, float> OnUnitDeath;
        public event Action<Unit> OnUnitLife;

        public Unit Create(GameObject prefab, Vector3 position, ISystemableEntity target, Army army)
        {
            
            var obj = Object.Instantiate(prefab, position, Quaternion.identity);
            var unit = obj.GetComponent<Unit>();
            if (unit == null)
            {
                Debug.LogError("[PlayerFactory] Prefab has no Unit component!");
                Object.Destroy(obj);
                return null;
            }
            unit.ArmyOwner = army;
            unit.UnitTeam = Team.Player;
            unit.Position = position;
            unit.Init();
            unit.SetSupremeTarget(target);
            unit.OnUnitDeath += UnitDeth;
            unit.OnAlive += UnitLife;
            return unit;
        }

        public void UnitDeth(Unit unit)
        {
            OnUnitDeath?.Invoke(unit, unit.RespawnTime);
            // unit.OnUnitDeath -= UnitDeth;
            // unit.CanRespawn;

        }

        public void UnitLife(Unit unit)
        {
            OnUnitLife?.Invoke(unit);
        }

        public void UnitRespawnProgress(Unit unit, float progress)
        {
            OnUnitDeath?.Invoke(unit, unit.RespawnTime);
        }
    }
}