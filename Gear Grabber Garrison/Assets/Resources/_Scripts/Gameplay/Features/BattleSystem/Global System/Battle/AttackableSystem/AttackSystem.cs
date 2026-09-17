using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class AttackSystem : I_System
    { 
        //общая часть
        public ISystemableEntity Owner { get; set; }
        public SystemTargets Targets { get; set; }
        
        public void SetOwner(ISystemableEntity owner) => Owner = owner;
        public void Reset() => Stat.Reset();

        
        //индивидуальная
        public SystemType Type { get; set; } = SystemType.AttackSystem;
        public AttackStat Stat { get; private set; }
        public I_Strategy Strategy { get; private set; }

        private AttackTargetSerch _attackTargetSerch = new AttackTargetSerch();
        public void Register()
        {
            var priority = TickPriorityHelper.FromTeam(Owner.UnitTeam);
            TickManager.Instance.RegisterTickable(Strategy as ISystemTickable, type: SystemType.AttackSystem, priority: priority);
        }

        public void Unregister()
        {
            TickManager.Instance.UnregisterTickable(Strategy as ISystemTickable);
            Stat?.Clear();
        }
        
        
        //статический конструктор
        public static AttackSystem Create(ReadOnlyAttributes attributes, ISystemableEntity owner)
        {
            var system = new AttackSystem();
            system.Targets = new SystemTargets();
            system.Owner = owner;
            system.Stat = new AttackStat(
                attributes.TryGetAttribute(StatType.AttackMaxDamage),
                attributes.TryGetAttribute(StatType.AttackSpeed),
                attributes.TryGetAttribute(StatType.AttackRange)
                ); 
            //тут нужен кейс на выбор стратегии по StatType.AttackClass
            system.Strategy = new MaleAttackStrategy(system, system.Stat);

            // if (owner != null)
            //     system.Targets.SetDefault(owner);
            return system;
        }
        
        
        
         
        public AttackType AttackType { get; private set; }
        public float AttackAnimationDuration { get; private set; }
        

        public void Init()
        {
            if (Owner == null)
            {
                Debug.LogError($"[AttackSystem.Init] Owner is NULL!");
                return;
            }

            if (!Targets.HasTargets)
                Targets.SetDefault(Owner);
        }

        

        public bool TryAttack(Unit unit, float distance, ref Unit target)
        {
            //стратегии нет => не можем атаковать
            if (Strategy == null) return false;
            //юнита игрока нет => не можем атаковать
            // if(unit == null) return false;
            if (unit?.IsDead ?? false) return false;
            //враг есть, он в области атаки => можем атаковать
            if ((target !=null) && (distance < Stat.AttackRange)) return true;
            
            // target = new Unit();
            target = _attackTargetSerch.EnemySearch(unit, distance);
            bool hasTarget = false;
            if (target != null) hasTarget = true;
            Stat.IsAttacking = hasTarget; 
            if (hasTarget)
                Stat.SetCurrentTarget(target);
            else
                Stat.ClearTarget();

            return hasTarget;
        }
    }
}