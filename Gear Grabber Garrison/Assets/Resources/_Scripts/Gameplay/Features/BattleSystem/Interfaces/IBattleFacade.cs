using System;

namespace Gameplay.Features.Battlesystem
{
    public interface IBattleFacade
    {
        event Action<Guid> OnEnemyDeath;
        event Action<Guid, float> OnPlayerDeath;
        event Action<Guid> OnPlayerLife;
        Unit GetUnit(Guid unitId);

        public void DoSystemTick(float deltaTime);
        
        public void CreatePlayerUnit(Guid guid);

        public void UpgradePlayerArmy(Guid guid, StatType stat, Action<float> valueChangeAction);
        public void UpgradePlayerArmy(Guid guid, StatType stat, Func<float, float> valueChangeAction);

    }
}