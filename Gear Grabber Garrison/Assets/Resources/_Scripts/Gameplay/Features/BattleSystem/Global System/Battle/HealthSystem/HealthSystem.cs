using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class HealthSystem : I_System
    { 
        //общая часть
        public ISystemableEntity Owner { get; set; }
        public SystemTargets Targets { get; set; }
        
        public void SetOwner(ISystemableEntity owner) => Owner = owner;
        public void Reset() => Stat.Revive();
        
        
        //индивидуальная
        public SystemType Type { get; set; } = SystemType.HealthSystem;
        public HealthStat Stat { get; private set; }
        public SimpleHealthStrategy SimpleHealthStrategy { get; private set; }
        
        public void Register()
        {
            var priority = TickPriorityHelper.FromTeam(Owner.UnitTeam);
            TickManager.Instance.RegisterTickable(SimpleHealthStrategy as ISystemTickable, type: SystemType.HealthSystem, priority: priority);
        }
        public void Unregister()
        {
            TickManager.Instance.UnregisterTickable(SimpleHealthStrategy as ISystemTickable);
            Stat?.Clear();
        }
        
        //статический конструктор
        public static HealthSystem Create(ReadOnlyAttributes attributes, ISystemableEntity owner)
        {
            var system = new HealthSystem();
            system.Targets = new SystemTargets();
            system.Owner = owner;
            system.Stat = new HealthStat(
                attributes.TryGetAttribute(StatType.HealthMax), 
                attributes.TryGetAttribute(StatType.HealthRegen), 
                attributes.TryGetAttribute(StatType.HealthRegenRate)
                ); 
            
            system.SimpleHealthStrategy = new SimpleHealthStrategy(system, system.Stat);
            // system.SimpleHealthStrategy.SetOwnerSystem(system);
            // system.SimpleHealthStrategy.SetStat(system.Stat);

            // if (owner != null)
            //     system.Targets.SetDefault(owner);
            return system;
        }
    }
}