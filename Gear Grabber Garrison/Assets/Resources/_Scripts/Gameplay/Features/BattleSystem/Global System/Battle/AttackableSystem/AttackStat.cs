using System;

namespace Gameplay.Features.Battlesystem
{
    public class AttackStat
    {
        //Статы
        private StatParam _maxDamage;
        private StatParam _attackSpeed;
        private StatParam _attackRange;

        public int MaxDamage => (int)_maxDamage.value;
        public float AttackSpeed => _attackSpeed?.value ?? 0.000001f;
        public float AttackRange => (float)_attackRange.value;

        
        //конструктор
        public AttackStat(StatParam attackMaxDamage, StatParam attackSpeed, StatParam attackRange)
        {
            _maxDamage = attackMaxDamage;
            _attackSpeed = attackSpeed;
            _attackRange =  attackRange;
        }
        
        
        // События
        public event Action OnAttackStarted;
        public event Action OnAttackHit;
        public event Action OnAttackEnded;
        
        public void AttackStarted() => OnAttackStarted?.Invoke();
        public void AttackEnded() =>OnAttackEnded?.Invoke();
        public void AttackHit() => OnAttackHit?.Invoke();
        
        
        // Свойства
        public bool IsEnable = true;
        public Unit CurrentTarget { get; private set; }

        
        public bool IsAttacking { get; set; }
        
        public float CooldownAccumulator { get; set; }
        public float AttackInterval { get; set; }

        public void SetCurrentTarget(Unit target)
        {
            CurrentTarget = target;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
            IsAttacking = false;
        }

        public void Clear()
        {
            // отписки событий. сбрасываются тут!
        }

        public void Reset()
        {
            //сброс системы. обнуление
        }
    }
}