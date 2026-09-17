using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class SimpleHealthStrategy : IHealing, ISystemTickable
    {
        private HealthStat _stat;
        // private I_System _owner;
        private float _tickAccumulator;
        private float _timeRegeneration => 1f / _stat.HealthRegenRate;

        public SimpleHealthStrategy(I_System owner, HealthStat stat)
        {
            _stat = stat;
            // _owner = owner;
        }
        public void SetStat(HealthStat stat)
        {
            _stat = stat;
        }

        public void SetOwnerSystem(I_System owner)
        {
            // _owner = owner;
        }

        public void OnTick(float dt)
        { 
            if (!_stat.IsEnable) return;
            // Debug.Log("[стратегия лечения]>>> maxhealth:" + _stat.MaxHealth+" / speed:" + _stat.HealthRegenRate+" / regenValue:" + _stat.HealthRegenValue);
            
            if (_stat.IsDead || _stat.IsFullHealth) 
                return;

            _tickAccumulator += dt;

            if (_tickAccumulator >= _timeRegeneration)
            {
                _stat.Heal(_stat.HealthRegenValue);
                _tickAccumulator -= _timeRegeneration;
            }
        }
    }
}