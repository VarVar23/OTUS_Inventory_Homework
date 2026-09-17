using System;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class StunSystem : I_System
    {
        [Inject] private TickManager  _tickManager;
        public SystemType Type { get; set; } = SystemType.StunSystem;
        public ReadOnlyAttributes Attributes { get; set; }
        public ISystemableEntity Owner { get; set; }
        public SystemTargets Targets { get; set; }

        public StunStat Stat { get; private set; }

        public event Action OnStunStarted;
        public event Action OnStunEnded;

        public static StunSystem Create(ReadOnlyAttributes attributes, ISystemableEntity owner, float baseDuration)
        {
            var system = new StunSystem();
            system.Attributes = attributes;
            system.Targets = new SystemTargets();
            system.Owner = owner;

            float resistance = attributes.TryGetAttribute(StatType.StunResistance)?.value ?? 0f;
            float actualDuration = baseDuration * (1f - resistance);

            if (actualDuration <= 0f)
                return null;

            system.Stat = new StunStat();
            system.Stat.Restart(actualDuration);
            system.Stat.OnStunStarted += () => system.OnStunStarted?.Invoke();
            system.Stat.OnStunEnded += () => system.OnStunEnded?.Invoke();

            return system;
        }

        public void Init()
        {
        }

        public void Register()
        {
            var priority = TickPriorityHelper.FromTeam(Owner.UnitTeam);
            _tickManager.RegisterTickable(this as ISystemTickable, type: SystemType.StunSystem, priority: priority);
        }

        public void Unregister()
        {
            _tickManager.UnregisterTickable(this as ISystemTickable);
            Stat?.Clear();
        }

        public void SetOwner(ISystemableEntity owner)
        {
            Owner = owner;
        }

        public void Restart(float baseDuration)
        {
            float resistance = Attributes.TryGetAttribute(StatType.StunResistance)?.value ?? 0f;
            float actualDuration = baseDuration * (1f - resistance);
            Stat?.Restart(actualDuration);
        }

        public void OnTick(float dt)
        {
            if ((Owner as Unit)?.IsDead ?? false)
            {
                SelfDestruct();
                return;
            }

            if (!Stat.IsStunned)
                return;

            Stat.RemainingTime -= dt;

            if (Stat.RemainingTime <= 0f)
            {
                Stat.End();
                SelfDestruct();
            }
        }

        private void SelfDestruct()
        {
            Stat?.Clear();
            Unregister();
            Owner?.Components?.DetachSystem(SystemType.StunSystem);
        }
    }
}