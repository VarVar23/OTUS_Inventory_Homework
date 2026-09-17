using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class MoveableSystem : I_System
    { 
        //общая часть
        public ISystemableEntity Owner { get; set; }
        public SystemTargets Targets { get; set; }
        
        public void SetOwner(ISystemableEntity owner) => Owner = owner;
        public void Reset() => Stat.Reset();


        //индивидуальная
        public SystemType Type { get; set; } = SystemType.MoveSystem;
        public MoveStat Stat { get; private set; }
        public I_Strategy Strategy { get; private set; }
        private DefaultMoveStrategy _strategy;
        public void Register()
        {
            var priority = TickPriorityHelper.FromTeam(Owner.UnitTeam);
            TickManager.Instance.RegisterTickable(_strategy, type: SystemType.MoveSystem, priority: priority);
        }

        public void Unregister()
        {
            TickManager.Instance.UnregisterTickable(_strategy);
            Stat.Clear(); // Обязательно отписываем события
        }

        //статический конструктор
        public static MoveableSystem Create(ReadOnlyAttributes attributes, ISystemableEntity owner)
        {
            var system = new MoveableSystem();
            system.Targets = new SystemTargets();
            system.Owner = owner;
            system.Stat = new MoveStat(
                attributes.TryGetAttribute(StatType.MoveSpeed),
                attributes.TryGetAttribute(StatType.MovePatrolRadius)
            ); 
            system.Strategy = new DefaultMoveStrategy(system, system.Stat);

            // if (owner != null)
            //     system.Targets.SetDefault(owner);
            
            // if (!Targets.HasTargets)
            //     Targets.SetDefault(Owner);
            return system;
        }
        
        
        public void SetSupremeTarget(Vector3 target)
        {
            _strategy?.SetSupremeTarget(target);
        }

           
        
        
        
        
        
    }
}