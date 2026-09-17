using System.Collections.Generic;

namespace Gameplay.Features.Battlesystem
{
    public class SystemTargets
    {
        private readonly List<(int priority, ISystemableEntity target)> _targets = new();
        private ISystemableEntity _default;
        
        // Кеши
        private List<ISystemableEntity> _cachedAll;
        private ISystemableEntity _cachedPrimary;
        private bool _dirty = true;
        
        public void SetDefault(ISystemableEntity defaultTarget)
        {
            _default = defaultTarget;
            _dirty = true;
        }
        
        public void Add(ISystemableEntity target, int priority = 0)
        {
            _targets.Add((priority, target));
            _dirty = true;
        }
        
        public void Remove(ISystemableEntity target)
        {
            _targets.RemoveAll(t => t.target == target);
            _dirty = true;
        }
        
        public void Clear()
        {
            _targets.Clear();
            _dirty = true;
        }
        
        public ISystemableEntity Primary
        {
            get
            {
                if (_dirty) RebuildCache();
                return _cachedPrimary ?? _default;
            }
        }
        
        public List<ISystemableEntity> All
        {
            get
            {
                if (_dirty) RebuildCache();
                return _cachedAll;
            }
        }
        
        public bool HasTargets => _targets.Count > 0;
        public int Count => _targets.Count;
        
        private void RebuildCache()
        {
            _cachedAll = new List<ISystemableEntity>();
            _cachedPrimary = null;
            
            if (_targets.Count == 0)
            {
                _dirty = false;
                return;
            }
            
            int minPriority = int.MaxValue;
            
            for (int i = 0; i < _targets.Count; i++)
            {
                var item = _targets[i];
                _cachedAll.Add(item.target);
                
                if (item.priority < minPriority)
                {
                    minPriority = item.priority;
                    _cachedPrimary = item.target;
                }
            }
            
            _dirty = false;
        }
    }
}