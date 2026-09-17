using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class SystemAttributes
    {
        private readonly Dictionary<StatType, StatParam> _attributes = new();
        public IReadOnlyDictionary<StatType, StatParam> Attributes => _attributes;

        public StatParam TryGetAttribute(StatType type)
        {
            return _attributes.TryGetValue(type, out var statParam) ? statParam : null;
        }

        public void Add(StatParam statParam)
        {
            _attributes.Add(statParam.type, statParam);
        }

        public void Remove(StatType type)
        {
            _attributes.Remove(type);
        }

        public void Overwrite(StatParam statParam)
        {
            _attributes.Remove(statParam.type);
            _attributes.Add(statParam.type, statParam);
        }

        public void ClearAll()
        {
            _attributes.Clear();
        }
        
        // Получить значение или вернуть default
        public float GetValue(StatType type, float defaultValue = 0f)
        {
            var attr = TryGetAttribute(type);
            return attr?.value ?? defaultValue;
        }
        
        // Установить значение (создать если нет), clamp ≥ 0 для защиты от отрицательных
        public void SetValue(StatType type, float value)
        {
            value = Mathf.Max(0f, value);

            var attr = TryGetAttribute(type);
            if (attr != null)
            {
                attr.value = value;
            }
            else
            {
                Add(new StatParam { type = type, value = value });
            }
        }
        
        // Проверить наличие атрибута
        public bool HasAttribute(StatType type)
        {
            return _attributes.ContainsKey(type);
        }
        
        // Получить или создать атрибут
        public StatParam GetOrCreate(StatType type, float defaultValue = 0f)
        {
            var attr = TryGetAttribute(type);
            if (attr == null)
            {
                attr = new StatParam { type = type, value = defaultValue };
                Add(attr);
            }
            return attr;
        }
      
    }
}