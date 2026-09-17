using System;
using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemOwnerRegistry
    {
        private readonly Dictionary<Guid, List<Item>> _equippedItems = new();
        private readonly Dictionary<Guid, Guid> _itemOwners = new();
        private readonly Dictionary<Guid, string> _ownerDescriptions = new();

        public void RegisterEquip(Guid ownerId, Item item)
        {
            if (item == null) return;

            if (_itemOwners.TryGetValue(item.Id, out Guid previousOwnerId) && previousOwnerId != ownerId)
            {
                if (_equippedItems.TryGetValue(previousOwnerId, out var previousItems) && previousItems.Remove(item))
                {
                    RebuildDescription(previousOwnerId);
                }
            }

            if (!_equippedItems.ContainsKey(ownerId))
                _equippedItems[ownerId] = new List<Item>();

            _itemOwners[item.Id] = ownerId;

            if (!_equippedItems[ownerId].Contains(item))
            {
                _equippedItems[ownerId].Add(item);
                RebuildDescription(ownerId);
            }
        }

        public void RegisterUnequip(Guid ownerId, Item item)
        {
            if (item == null) return;

            if (_equippedItems.TryGetValue(ownerId, out var items))
            {
                if (items.Remove(item))
                {
                    RebuildDescription(ownerId);
                }
            }

            if (_itemOwners.TryGetValue(item.Id, out Guid mappedOwnerId) && mappedOwnerId == ownerId)
            {
                _itemOwners.Remove(item.Id);
            }
        }

        public bool TryGetItemOwner(Guid itemId, out Guid ownerId) => _itemOwners.TryGetValue(itemId, out ownerId);

        public string GetDescription(Guid ownerId) => _ownerDescriptions.GetValueOrDefault(ownerId, "No items equipped.");

        private void RebuildDescription(Guid ownerId)
        {
            List<Item> items = _equippedItems.GetValueOrDefault(ownerId);
            
            if (items == null || items.Count == 0)
            {
                _ownerDescriptions[ownerId] = "No items equipped.";
                return;
            }

            _ownerDescriptions[ownerId] = DescriptionGenerator.GenerateOwnerSummary(items);
        }
    }
}