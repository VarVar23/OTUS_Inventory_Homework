using System.Collections.Generic;
using System.Text;

namespace Gameplay.Features.Battlesystem
{
    public class SystemableComponents 
    {
        private ISystemableEntity _owner;
        public SystemableComponents(ISystemableEntity owner)
        {
            _owner = owner;
        }
        private readonly Dictionary<SystemType, I_System> _systems = new();
        public IReadOnlyDictionary<SystemType, I_System> Systems => _systems;

        public string allList { get; private set; } = "Emptyyy`1";

        public I_System TryGetSystem(SystemType Type) 
        {
            return _systems.TryGetValue(Type, out var system) ? system : null;
        }
        
        public void AttachSystem(I_System system)
        {
            system.Owner = _owner;
            system.Targets = new SystemTargets();
            system.Register();
            _systems.Add(system.Type, system);
        }
        
        public void DetachSystem(I_System system)
        {
            if (system != null && _systems.ContainsKey(system.Type))
            {
                system.Unregister();
                _systems.Remove(system.Type);
            }
        }

        public void DetachSystem(SystemType type)
        {
            if (_systems.TryGetValue(type, out var system))
            {
                system.Unregister();
                _systems.Remove(type);
            }
        }

        public void Overwrite(I_System system)
        {
                _systems.Remove(system.Type);
                _systems.Add(system.Type, system);
        }

        public void Clear()
        {
            // Сначала отписываем все системы
            foreach (var system in _systems.Values)
            {
                system?.Unregister();
            }
            // Только потом очищаем список
            _systems.Clear();
        }
        
        public string GetComponentsList()
        {
            if (_systems.Count == 0)
                return "No systems attached";
            
            var sb = new StringBuilder();
            sb.AppendLine($"Systems ({_systems.Count}):");
            foreach (var kvp in _systems)
            {
                sb.AppendLine($"  - {kvp.Key}");
            }
            return sb.ToString();
        }
    }
}