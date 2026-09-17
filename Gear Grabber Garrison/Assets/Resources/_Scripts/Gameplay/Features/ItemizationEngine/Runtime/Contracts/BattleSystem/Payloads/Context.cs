using System.Collections.Generic;

namespace Gameplay.Itemization
{
    public abstract class Context
    {
        private Dictionary<string, float> _lockedData = new();
        private readonly Dictionary<string, float> _volatileData = new();

        public void LoadLockedData(Dictionary<string, float> data) => _lockedData = data;

        protected void ImportVolatileData(Context other)
        {
            foreach (var pair in other.GetVolatileData())
                _volatileData[pair.Key] = pair.Value;
        }

        public virtual IEnumerable<KeyValuePair<string, float>> GetVolatileData() => _volatileData;

        public float Get(string key)
        {
            if (_volatileData.TryGetValue(key, out var volatileValue)) return volatileValue;
            if (_lockedData.TryGetValue(key, out var lockedValue)) return lockedValue;
            return 0;
        }

        public void SetVolatile(string key, float value) => _volatileData[key] = value;

        public void ResetVolatile() => _volatileData.Clear();

        // VolatileData is available for rule-chain transforms that need to pass intermediate
        // computed values downstream without permanently affecting the locked attribute data.
    }
}