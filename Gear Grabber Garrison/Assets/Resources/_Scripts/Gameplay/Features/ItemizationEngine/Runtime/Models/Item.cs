using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Itemization.Models
{
    internal sealed class Item : IItem
    {
        public Guid Id { get; private set; }
        public string ItemName { get; private set; }
        public ItemType Type { get; private set; }
        public Sprite Icon { get; private set; }
        public string Description { get; private set; }
        public string UpgradeDescription { get; private set; }
        public bool CanUpgrade { get; private set; }

        internal List<ItemAttribute> UpgradeableAttributes { get; private set; } = new();
        internal IReadOnlyList<ItemAttribute> Attributes => _attributes;
        private readonly List<ItemAttribute> _attributes;

        public ItemValueData ValueData { get; private set; }

        internal Item(string itemName, ItemType type, Sprite icon, List<ItemAttribute> attributes)
        {
            Id = Guid.NewGuid();
            ItemName = itemName;
            Type = type;
            Icon = icon;
            _attributes = attributes ?? new List<ItemAttribute>();

            for (int i = 0; i < _attributes.Count; i++)
            {
                _attributes[i].OnAttributeLevelUpdate += HandleAttributeUpdate;
            }

            UpgradeableAttributes = new List<ItemAttribute>();
            for (int i = 0; i < _attributes.Count; i++)
            {
                if (_attributes[i].CanUpgrade)
                    UpgradeableAttributes.Add(_attributes[i]);
            }
            CanUpgrade = UpgradeableAttributes.Count > 0;
        }

        internal void UpdateDescriptions(string desc, string upgradeDesc) 
        { 
            Description = desc; 
            UpgradeDescription = upgradeDesc; 
        }
        internal void UpdateValueData(ItemValueData valueData) => ValueData = valueData;

        private void HandleAttributeUpdate(ItemAttribute updatedAttribute)
        {
            if (!updatedAttribute.CanUpgrade)
            {
                UpgradeableAttributes.Remove(updatedAttribute);
                CanUpgrade = UpgradeableAttributes.Count > 0;
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < _attributes.Count; i++)
            {
                _attributes[i].OnAttributeLevelUpdate -= HandleAttributeUpdate;
            }
        }
    }
}

