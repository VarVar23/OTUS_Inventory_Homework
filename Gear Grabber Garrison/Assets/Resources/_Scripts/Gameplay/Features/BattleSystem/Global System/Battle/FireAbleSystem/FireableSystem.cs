using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class FireableSystem : I_System
    {
        [Inject] private TickManager  _tickManager;
        public SystemType Type { get; set; } = SystemType.FireSystem;
        public ReadOnlyAttributes Attributes { get; set; }
        public ISystemableEntity Owner { get; set; }
        public SystemTargets Targets { get; set; }

        private FireStrategy _strategy;
        private FireStat _stat;

        public static FireableSystem Create(ReadOnlyAttributes attributes, ISystemableEntity target = null)
        {
            var system = new FireableSystem();
            system.Attributes = attributes;
            system.Targets = new SystemTargets();

            var fireStrange = attributes.TryGetAttribute(StatType.FireStrange);
            var fireTime = attributes.TryGetAttribute(StatType.FireTime);

            float strangeValue = fireStrange?.value ?? 10f;
            float timeValue = fireTime?.value ?? 4f;

            system._stat = new FireStat(strangeValue, timeValue);

            if (target != null)
                system.Targets.Add(target);

            return system;
        }

        public void Init()
        {
            if (Owner == null)
            {
                Debug.LogError($"[FireableSystem.Init] Owner is NULL!");
                return;
            }

            _strategy = new FireStrategy(Attributes, this);

            if (!Targets.HasTargets)
                Targets.SetDefault(Owner);
        }

        public void Register()
        {
            // _tickManager.
        }

        public void Unregister()
        {
            _stat?.Clear();
        }

        public void SetOwner(ISystemableEntity owner)
        {
            Owner = owner;
        }

        public bool TryTick(float dt)
        {
            return true;
        }
    }
}