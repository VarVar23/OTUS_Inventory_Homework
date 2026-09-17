using System;
using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal sealed class AttributeEffectManager
    {
        private readonly ItemOwnerRegistry _ownerRegistry;
        private readonly Dictionary<Guid, IReadOnlyList<ItemAttribute>> _itemAttributes = new();

        public AttributeEffectManager(ItemOwnerRegistry ownerRegistry)
        {
            _ownerRegistry = ownerRegistry;
        }

        public IReadOnlyList<ResolutionContext> RegisterItemAndResolve(
            Guid ownerId,
            Guid itemId,
            IReadOnlyList<ItemAttribute> attributes)
        {
            if (_itemAttributes.ContainsKey(itemId))
                return Array.Empty<ResolutionContext>();

            _itemAttributes[itemId] = attributes;

            List<ResolutionContext> output = new();
            TriggerContext context = new()
            {
                ItemOwnerId = ownerId,
                TargetId = ownerId,
                Trigger = AttributeTrigger.OnItemEquipped
            };

            for (int i = 0; i < attributes.Count; i++)
            {
                List<ResolutionContext> attributeOutput = attributes[i].ResolveTrigger(context);
                if (attributeOutput != null && attributeOutput.Count > 0)
                    output.AddRange(attributeOutput);
            }

            return output;
        }

        public IReadOnlyList<ResolutionContext> UnregisterItemAndResolve(Guid ownerId, Guid itemId)
        {
            if (!_itemAttributes.TryGetValue(itemId, out var attributes))
                return Array.Empty<ResolutionContext>();

            List<ResolutionContext> output = new();
            TriggerContext context = new()
            {
                ItemOwnerId = ownerId,
                TargetId = ownerId,
                Trigger = AttributeTrigger.OnItemUnequipped
            };

            for (int i = 0; i < attributes.Count; i++)
            {
                List<ResolutionContext> attributeOutput = attributes[i].ResolveTrigger(context);
                if (attributeOutput != null && attributeOutput.Count > 0)
                    output.AddRange(attributeOutput);
            }

            _itemAttributes.Remove(itemId);
            return output;
        }

        public IReadOnlyList<ResolutionContext> ActivateTrigger(TriggerContext context)
        {
            List<ResolutionContext> output = new();

            foreach (var (itemId, attributes) in _itemAttributes)
            {
                if (!_ownerRegistry.TryGetItemOwner(itemId, out Guid owner) || owner != context.ItemOwnerId)
                    continue;

                for (int j = 0; j < attributes.Count; j++)
                {
                    List<ResolutionContext> attrOutput = attributes[j].ResolveTrigger(context);
                    if (attrOutput != null && attrOutput.Count > 0)
                        output.AddRange(attrOutput);
                }
            }

            return output;
        }
    }
}
