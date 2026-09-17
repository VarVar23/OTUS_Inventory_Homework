using System;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class HealthStat
    {
        //Статы
        private StatParam _baseMaxHealth;
        private StatParam _healthRegenRate;
        private StatParam _healthRegenValue;
        
        public int MaxHealth => (int)(_baseMaxHealth?.value ?? 1f);
        public float HealthRegenRate => _healthRegenRate?.value ?? 0.00001f;
        public int HealthRegenValue => (int)(_healthRegenValue?.value ?? 0f);
       
        
        //конструктор
        public HealthStat(StatParam maxHealthAttr, StatParam regenValueAttr, StatParam regenSpeedAttr)
        {
            _baseMaxHealth = maxHealthAttr;
            _healthRegenRate = regenSpeedAttr;
            _healthRegenValue =  regenValueAttr;
        }
        
        
        // События
        public event Action OnHealthChanged;
        public event Action<int> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnAlive;
        
        
        // Свойства
        public bool IsEnable = true;
        public bool IsDead => _currentHealth <= 0;
        private bool _wasDead;
        public int MissingHealth => MaxHealth - CurrentHealth;
        public bool IsFullHealth => CurrentHealth >= MaxHealth;
        
        private float _regenAccumulator;
        
        private float _currentHealth;
        public int CurrentHealth => (int)_currentHealth;
        
        
        //Методы
     
        
        public void TakeDamage(int damage)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnDamageTaken?.Invoke(damage);
            
            if (_currentHealth <= 0)
                OnDeath?.Invoke();
            
            HealthChanged();
        }
        
        public void Heal(int amount)
        {
            _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
            HealthChanged();
        }
        
        public void SetHealth(int health)
        {
            _currentHealth = Mathf.Clamp(health, 0, MaxHealth);
            HealthChanged();
        }
        
        public void Revive()
        {
            OnAlive?.Invoke();
            _currentHealth = MaxHealth;
            _regenAccumulator = 0f;
        }

        public void HealthChanged()
        {
            OnHealthChanged?.Invoke();
        }

        public void Clear()
        {
            OnHealthChanged = null;
            OnDamageTaken = null;
            OnDeath = null;
            OnAlive = null;
        }
    }
}