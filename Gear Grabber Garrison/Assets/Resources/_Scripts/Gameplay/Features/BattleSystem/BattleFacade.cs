
using System;
using Zenject;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class BattleFacade : IBattleFacade, IInitializable
    {
        [Inject] private TickManager _tickManager;
        [Inject] private BattleManager _manager; 
        
        public void Initialize()
        {
            _tickManager = new TickManager();
            _manager.EnemyFactory.OnUnitDeath += HandleEnemyDeath;
            _manager.PlayerFactory.OnUnitDeath += HandlePlayerDeath;
            _manager.PlayerFactory.OnUnitLife += HandlePlayerLife;
        }
        
        public event Action<Guid> OnEnemyDeath;
        public event Action<Guid> OnPlayerLife;
        public event Action<Guid, float> OnPlayerDeath;

        public void DoSystemTick(float deltaTime)
        {
            _tickManager.DoTick(deltaTime);
        }
        
        public void CreatePlayerUnit(Guid guid)
        {
            _manager.CreatePlayerArmy(guid);
        }
        
        public void UpgradePlayerArmy(Guid guid, StatType stat, Func<float,float> valueChangeAction)
        {
            var army = GetPlayerArmy(guid);
            army.UpgradeAllUnits(stat, valueChangeAction);
        }
        public void UpgradePlayerArmy(Guid guid, StatType stat, Action<float> valueChangeAction)
        {
            var army = GetPlayerArmy(guid);
            // army.UpgradeAllUnits(stat, valueChangeAction);
        }
        
        public Army GetPlayerArmy(Guid guid)
        {
            return _manager?.GetPlayerArmy(guid);
        }
        
        public Unit GetUnit(Guid unitId)
        {
            return UnitRegistry.GetByGuid(unitId);
        }
        
        private void HandlePlayerDeath(Unit unit, float time)
        {
            Debug.Log("[BattleFassade] Player Unit dead! respavn = " + time + " cек");
            OnPlayerDeath?.Invoke(unit.ArmyOwner.Id, time);
            Debug.Log("[BattleFassade] Army Killed!" + unit.ArmyOwner.Id);
        }

        private void HandlePlayerLife(Unit unit)
        {
            Debug.Log("[BattleFassade] Player Unit alive!");
            OnPlayerLife?.Invoke(unit.ArmyOwner.Id);
        }

        private void HandleEnemyDeath(Unit unit)
        {
            Debug.Log("[BattleFassade] Enemy dead!");
            OnEnemyDeath?.Invoke(unit.Id);
        }
    }
}