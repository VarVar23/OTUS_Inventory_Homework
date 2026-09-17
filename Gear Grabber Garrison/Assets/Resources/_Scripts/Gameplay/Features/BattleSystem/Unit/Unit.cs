using System;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class Unit : MonoBehaviour, ISystemableEntity, ISystemTickable, IDamageable
    {
        [SerializeField] private UnitAttributesConfig _unitAttributesConfig;
        [SerializeField] private bool _canRespawn = true;
        
        public Army ArmyOwner; //в какой юнит армии? вот от этого зависит команда и guid и способности к респауну.
        public Guid Id { get; private set; }
        public Team UnitTeam { get; set; }
        
        public bool CanRespawn => _canRespawn;
        public float RespawnTime;
        
        //Позиционирование
        public Vector3 Position { get; set; }
        private Vector3 _lastPosition;

        //Атрибуты
        private SystemAttributes _attributes  { get; set; }
        public ReadOnlyAttributes Attributes =>
            _readOnlyCache ??= new ReadOnlyAttributes(_attributes);
        private ReadOnlyAttributes _readOnlyCache;
        public SystemAttributes GetWritableAttributes() => _attributes;
        public void ClearAttributeCache()
        {
            _readOnlyCache = null;
        }
        
        //Системы
        public SystemableComponents Components { get; set; }

        public const float HideAfterDeathDelay = 10f;

        
        public event Action<Unit> OnUnitDeath;
        public event Action<Unit> OnRespawnStarted;
        public event Action<Unit> OnAlive;

        public HealthSystem HealthSystem => _healthSystem;
        private HealthSystem _healthSystem;

        public bool IsDead => _healthSystem?.Stat.IsDead ?? true;
        public bool IsAlive => !IsDead;
        public bool IsStunned => (Components.TryGetSystem(SystemType.StunSystem) as StunSystem)?.Stat.IsStunned ?? false;

        public AttackSystem AttackSystem => _attackSystem;
        private AttackSystem _attackSystem;

        public MoveableSystem MoveSystem => _moveSystem;
        private MoveableSystem _moveSystem;

        private ISystemableEntity _supremeTarget;

        private bool _systemsInitialized;

        public void Init()
        {
            Id = Guid.NewGuid();
            if (_unitAttributesConfig == null) _unitAttributesConfig = new UnitAttributesConfig();
            _attributes = _unitAttributesConfig.BuildAttributes();
            
            _readOnlyCache = null;

            Components = new SystemableComponents(this);
            
            _moveSystem = MoveableSystem.Create(Attributes, this);
            _attackSystem = AttackSystem.Create(Attributes, this);
            _healthSystem = HealthSystem.Create(Attributes, this);
            _healthSystem.Stat.OnDeath += DisableUnit;

            Components.AttachSystem(_healthSystem);
            Components.AttachSystem(_attackSystem);
            Components.AttachSystem(_moveSystem);
            
            TickManager.Instance.RegisterTickable(this);
            UnitRegistry.Register(this);
            _systemsInitialized = false;
            Enable();
        }

        public void Enable()
        {
            name = "["+UnitTeam+"](attack="+Attributes.GetValue(StatType.AttackMaxDamage)+",health="+Attributes.GetValue(StatType.HealthMax)+")";
            OnAlive?.Invoke(this);

            CancelHide();
            gameObject.SetActive(true);

            Components.AttachSystem(FireableSystem.Create(Attributes, this));

            _healthSystem.Stat.Revive();
            SetSupremeTarget(_supremeTarget as ISystemableEntity);
        }

        public void DisableUnit()
        {
            Components.DetachSystem(SystemType.FireSystem);
            
            OnUnitDeath?.Invoke(this);
            if (CanRespawn)
            {
                OnRespawnStarted?.Invoke(this);
                ScheduleHide();
            }
            else
            {
                Components.Clear();
                UnitRegistry.Unregister(this);
                Destroy(gameObject, HideAfterDeathDelay);
            }
        }

        public void ScheduleHide()
        {
            Invoke(nameof(Hide), HideAfterDeathDelay);
        }

        public void Hide()
        {
            if (IsDead)
                gameObject.SetActive(false);
        }

        public void CancelHide()
        {
            CancelInvoke(nameof(Hide));
        }

        public void OnTick(float dt)
        {
            if (_lastPosition != Position)
                SetTransformPosition(Position);

            if (IsDead) return;

            // _attackSystem.TryAttack(this);
        }

        public void SetSupremeTarget(ISystemableEntity target)
        {
            _supremeTarget = target;
            if (target == null) return;
            var moveSystem = Components.TryGetSystem(SystemType.MoveSystem) as MoveableSystem;
            if (moveSystem != null)
                moveSystem.SetSupremeTarget(target.Position);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            _healthSystem.Stat.TakeDamage(damage);    
        }

        public bool CanHeal => !IsDead;

        public void SetTransformPosition(Vector3 position)
        {
            _lastPosition = position;
            transform.position = position;
        }

        public void LookAt(Vector3 target)
        {
            if (transform == null) return;

            Vector3 direction = target - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }

        private void OnDestroy()
        {
            //ClearUnitDeathEvent();
            if (Components != null)
            {
                Components.Clear();
            }
        }
    }
}