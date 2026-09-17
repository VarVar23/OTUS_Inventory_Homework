using System.Collections.Generic;
using Gameplay.Itemization.Models;
using UnityEngine;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemIconPicker
    {
        private readonly JSONItemizationRegistry _registry;
        private readonly Dictionary<string, Sprite> _iconCache = new();

        [Inject]
        public ItemIconPicker(JSONItemizationRegistry registry)
        {
            _registry = registry;
        }

        public Sprite GetIcon(string baseName)
        {
            if (_iconCache.TryGetValue(baseName, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            ItemIconConfigModel config = _registry.GetIconConfig();
            if (config == null) return null;

            IconMapping mapping = config.Mappings.Find(m => m.BaseName == baseName);
            if (mapping == null || string.IsNullOrEmpty(mapping.SpritePath))
            {
                Debug.LogWarning($"[IconPicker] No mapping or path found for base name: {baseName}");
                return null;
            }

            Sprite loadedSprite = Resources.Load<Sprite>(mapping.SpritePath);

            if (loadedSprite != null)
            {
                _iconCache.Add(baseName, loadedSprite);
            }
            else
            {
                Debug.LogError($"[IconPicker] Failed to load sprite at path: {mapping.SpritePath}");
            }

            return loadedSprite;
        }

        public void ClearCache()
        {
            _iconCache.Clear();
        }
    }
}