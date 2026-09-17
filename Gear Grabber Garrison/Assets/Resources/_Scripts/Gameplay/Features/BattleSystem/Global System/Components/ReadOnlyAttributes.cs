using System.Collections.Generic;

namespace Gameplay.Features.Battlesystem
{
    public class ReadOnlyAttributes
    {
        private readonly SystemAttributes _inner;
        // private readonly object _owner; // для отладки

        public ReadOnlyAttributes(SystemAttributes inner)
        {
            _inner = inner;
            // _owner = owner;
        }

        public StatParam TryGetAttribute(StatType type) => _inner.TryGetAttribute(type);
        
        public float GetValue(StatType type, float defaultValue = 0f) => 
            _inner.GetValue(type, defaultValue);
        
        public bool HasAttribute(StatType type) => _inner.HasAttribute(type);
        
        public IReadOnlyDictionary<StatType, StatParam> AllAttributes => _inner.Attributes;
        
        // Нет методов для записи!
        // Системы могут только ЧИТАТЬ атрибуты
    }
}