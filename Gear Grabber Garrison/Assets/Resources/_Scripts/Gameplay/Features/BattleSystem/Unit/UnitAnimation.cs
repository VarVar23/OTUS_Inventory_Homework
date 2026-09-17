using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
[RequireComponent(typeof(Unit))]
    public class UnitAnimation : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        
        // Animator parameter names (как в оригинальном Unit.cs)
        private static readonly int _isIdle = Animator.StringToHash("IsIdle");
        private static readonly int IsMoving = Animator.StringToHash("IsRun");
        private static readonly int IsAttacking = Animator.StringToHash("isAttack");
        private static readonly int IsCasting = Animator.StringToHash("IsCasting");
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int _isDamage = Animator.StringToHash("IsDamage");
        
        private Unit _unit;
        private AttackSystem _attackSystem;
        private MoveableSystem _moveableSystem;
        private StunSystem _stunSystem;
        private HealthSystem _healthSystem;
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
            
            // Если аниматор не назначен в инспекторе, ищем во вложенных объектах
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
                if (_animator == null)
                {
                    Debug.LogWarning($"[{nameof(UnitAnimation)}] Animator не найден на {gameObject.name} и в дочерних объектах!");
                }
            }
        }
        
        private void Start()
        {
            _attackSystem = _unit.Components.TryGetSystem(SystemType.AttackSystem) as AttackSystem;
            _moveableSystem = _unit.Components.TryGetSystem(SystemType.MoveSystem) as MoveableSystem;
            _healthSystem = _unit.HealthSystem;
            _stunSystem = _unit.Components.TryGetSystem(SystemType.StunSystem) as StunSystem;
            
            // Подписка на события движения
            if (_moveableSystem != null)
            {
                _moveableSystem.Stat.OnMovementStarted += HandleMovementStarted;
                _moveableSystem.Stat.OnMovementStopped += HandleMovementStopped;
            }
            
            // Подписка на события атаки
            if (_attackSystem != null)
            {
                _attackSystem.Stat.OnAttackStarted += HandleAttackStarted;
                _attackSystem.Stat.OnAttackHit += HandleAttackHit;
                _attackSystem.Stat.OnAttackEnded += HandleAttackEnded;
            }
            
            // Подписка на события оглушения
            if (_stunSystem != null)
            {
                _stunSystem.OnStunStarted += HandleStunStarted;
                _stunSystem.OnStunEnded += HandleStunEnded;
            }
            
            // Подписка на событие health
            if (_healthSystem != null)
            {
                _healthSystem.Stat.OnDamageTaken += HandleDamageTaken;
                _healthSystem.Stat.OnDeath += HandleDeath;
                _healthSystem.Stat.OnAlive += HandleAlive;
            }
            
            // Подписка на паузу
            TickManager.Instance.OnPauseChanged += HandlePauseChanged;
        }
        
        private void OnDestroy()
        {
            if (_moveableSystem != null)
            {
                _moveableSystem.Stat.OnMovementStarted -= HandleMovementStarted;
                _moveableSystem.Stat.OnMovementStopped -= HandleMovementStopped;
            }
            
            if (_attackSystem != null)
            {
                _attackSystem.Stat.OnAttackStarted -= HandleAttackStarted;
                _attackSystem.Stat.OnAttackHit -= HandleAttackHit;
                _attackSystem.Stat.OnAttackEnded -= HandleAttackEnded;
            }
            
            if (_stunSystem != null)
            {
                _stunSystem.OnStunStarted -= HandleStunStarted;
                _stunSystem.OnStunEnded -= HandleStunEnded;
            }
            
            if (_healthSystem != null)
            {
                _healthSystem.Stat.OnDamageTaken -= HandleDamageTaken;
                _healthSystem.Stat.OnDeath -= HandleDeath;
                _healthSystem.Stat.OnAlive -= HandleAlive;
            }
            
            if (TickManager.Instance != null)
            {
                TickManager.Instance.OnPauseChanged -= HandlePauseChanged;
            }
        }
        
        // === Movement Events ===
        
        private void HandleMovementStarted()
        {
            if (_animator == null) return;
            _animator.SetBool(IsMoving, true);
        }
        
        private void HandleMovementStopped()
        {
            if (_animator == null) return;
            _animator.SetBool(IsMoving, false);
        }
        
        // === Attack Events ===
        
        private void HandleAttackStarted()
        {
            if (_animator == null) return;
            
            // Синхронизация скорости атаки с анимацией
            float attackInterval = 1f / _attackSystem.Stat.AttackSpeed;
            float baseAnimationDuration = _attackSystem.AttackAnimationDuration;
            float animationSpeed = baseAnimationDuration / attackInterval;
            
            _animator.SetFloat(AttackSpeed, animationSpeed);
            
            // Разные триггеры для разных типов атаки
            if (_attackSystem.AttackType == AttackType.Melee)
            {
                _animator.SetTrigger("Attack");
            }
            else if (_attackSystem.AttackType == AttackType.Magic)
            {
                _animator.SetBool(IsCasting, true);
            }
        }
        
        private void HandleAttackHit()
        {
            if (_animator == null) return;
            
            // Для магии: здесь можно создать визуальный эффект (молнию)
            // Этот метод вызывается когда наносится урон
            // Можно использовать для синхронизации эффектов
        }
        
        private void HandleAttackEnded()
        {
            if (_animator == null) return;
            
            if (_attackSystem.AttackType == AttackType.Magic)
            {
                _animator.SetBool(IsCasting, false);
            }
            
            _animator.SetBool(IsAttacking, false);
        }
        
        // === Stun Events ===
        
        private void HandleStunStarted()
        {
            if (_animator == null) return;
            _animator.SetBool(IsStunned, true);
        }
        
        private void HandleStunEnded()
        {
            if (_animator == null) return;
            _animator.SetBool(IsStunned, false);
        }
        
        // === Damage Events ===
        
        private void HandleDamageTaken(int damage)
        {
            if (_animator == null) return;
            _animator.SetTrigger(_isDamage);
            
            // Можно добавить эффект красной вспышки
            // StartCoroutine(FlashRed());
        }
        
        // === Animation Events (вызываются из Unity Animator) ===
        
        // Этот метод вызывается через Animation Event в момент удара
        public void OnAnimationHit()
        {
            // Можно добавить звук, эффекты, экран shake
            // Анимация уведомляет, что удар произошёл
        }
        
        //Этот метод вызывается через Animation Event в конце анимации атаки
        public void OnAnimationAttackEnd()
        {
            if (_animator == null) return;
            _animator.SetBool(IsAttacking, false);
        }
        
        // === Public API ===

        private void HandleAlive()
        { 
            if (_animator == null) return;
            _animator.SetTrigger(_isIdle);
        }
        private void HandleDeath()
        {
            if (_animator == null) return;
            _animator.SetTrigger(IsDead);
        }
        
        public void SetMoveSpeed(float speed)
        {
            if (_animator == null) return;
            _animator.SetFloat(MoveSpeed, speed);
        }
        
        private void HandlePauseChanged(bool isPaused)
        {
            if (_animator == null) return;
            _animator.speed = isPaused ? 0 : 1;
        }
    }
}