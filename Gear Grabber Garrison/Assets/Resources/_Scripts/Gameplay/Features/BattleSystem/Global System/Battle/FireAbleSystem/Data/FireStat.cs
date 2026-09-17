using System;

namespace Gameplay.Features.Battlesystem
{
    public class FireStat
    {
        // Базовые значения
        #region Fields

        public StatParam FireStrange { get; private set; } 
        public StatParam FireTime{get; private set;}  
    
        #endregion
        
        // События
        public event Action OnHealthChanged;
        public event Action<int> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnAlive;
        
        
        // Свойства
        public bool IsEnable = true;
        public bool IsFireEnded = false;
        
        
        //Методы
        public FireStat(float fireStrange , float fireTime)
        {
            Change(fireStrange, fireTime);
        }

        public void Change(float fireStrange, float fireTime)
        {
            if(FireStrange == null)
                FireStrange = new StatParam();
            FireStrange.value = fireStrange;
            
            if(FireTime == null)
                FireTime = new StatParam();
            FireTime.value = fireTime;
        }
        
        
        public void Revive()
        {
            //
            // OnAlive?.Invoke();
            // _currentHealth = MaxHealth;
            // _regenAccumulator = 0f;
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
        
        // // Модификаторы (на потом)
        // private List<int> _maxHealthModifiers = new List<int>();
        //
        // // Модификаторы (на потом - заглушки)
        // public void AddMaxHealthModifier(int value)
        // {
        //     _maxHealthModifiers.Add(value);
        // }
        //
        // public void RemoveMaxHealthModifier(int value)
        // {
        //     _maxHealthModifiers.Remove(value);
        // }
    }
}