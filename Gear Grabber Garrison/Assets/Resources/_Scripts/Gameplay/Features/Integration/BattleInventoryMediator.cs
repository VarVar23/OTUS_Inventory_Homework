using Gameplay.Features.Battlesystem;
using Gameplay.Inventory;
using Gameplay.TimeSystem;
using System;
using Zenject;

namespace Gameplay.Integration
{
    public class BattleInventoryMediator : IInitializable, IDisposable
    {
        [Inject] private IBattleFacade _battle;
        [Inject] private IInventory _inventory;
        [Inject] private ITimeFacade _time;

        public void Initialize()
        {
            _battle.OnPlayerDeath += HandlePlayerDeath;
            _battle.OnPlayerLife += HandlePlayerRespawn;
            _inventory.Unit.OnUnitCreated += HandleUnitCreated;
            _time.OnTick += HandleTick;
        }

        public void Dispose()
        {
            _battle.OnPlayerDeath -= HandlePlayerDeath;
            _battle.OnPlayerLife -= HandlePlayerRespawn;
            _inventory.Unit.OnUnitCreated -= HandleUnitCreated;
            _time.OnTick -= HandleTick;
        }

        private void HandleTick()
        {
            _inventory.Unit.Tick(_time.DeltaTime);
        }

        private void HandleUnitCreated(Guid guid)
        {
            _battle.CreatePlayerUnit(guid);
        }

        private void HandlePlayerDeath(Guid guid, float reloadSeconds)
        {
            _inventory.Unit.StartReload(guid, reloadSeconds);
        }

        private void HandlePlayerRespawn(Guid guid)
        {
            _inventory.Unit.StopReload(guid);
        }
    }
}