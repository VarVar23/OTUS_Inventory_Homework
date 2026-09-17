using System;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public interface IUnitFactory
    {
        // public event Action<Unit> OnUnitDeath;


        public Unit Create(GameObject prefab, Vector3 position, ISystemableEntity target, Army army);
        public void UnitDeth(Unit unit);
    }
}