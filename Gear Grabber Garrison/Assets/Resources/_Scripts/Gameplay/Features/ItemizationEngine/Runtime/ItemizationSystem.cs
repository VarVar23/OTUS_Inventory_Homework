using System;
using System.Collections.Generic;
using Gameplay.Itemization.Models;
using Gameplay.Itemization.Services;
using Zenject;
using UnityEngine;

namespace Gameplay.Itemization
{
    public sealed class ItemizationSystem : IItemizationSystem
    {
        [Inject] private readonly ItemPipelineManager _pipeline;
        [Inject] private readonly AttributeEffectManager _effects;
        [Inject] private readonly ItemOwnerRegistry _ownerRegistry;

        #region General Sockets
        public IItem GetItemData(Guid itemId) => _pipeline.GetItemData(itemId);
        #endregion

        #region Inventory System Sockets
        public event Action<int> OnUnclaimedQueueUpdate
        {
            add => _pipeline.OnUnclaimedQueueUpdate += value;
            remove => _pipeline.OnUnclaimedQueueUpdate -= value;
        }

        public event Action<IReadOnlyList<IResolutionPayload>> OnInventoryResolved;

        public void EquipItem(Guid ownerId, Guid itemId)
        {
            Item item = _pipeline.GetItemData(itemId);
            if (item == null) return;

            _ownerRegistry.RegisterEquip(ownerId, item);

            IReadOnlyList<ResolutionContext> internalResults =
                _effects.RegisterItemAndResolve(ownerId, itemId, item.Attributes);

            PublishInventoryResolution(internalResults);
        }

        public void UnequipItem(Guid ownerId, Guid itemId)
        {
            Item item = _pipeline.GetItemData(itemId);
            if (item == null) return;

            IReadOnlyList<ResolutionContext> internalResults =
                _effects.UnregisterItemAndResolve(ownerId, itemId);

            PublishInventoryResolution(internalResults);
            _ownerRegistry.RegisterUnequip(ownerId, item);
        }
        
        public IItem CraftFromLootTable(LootTable table)
        {
            string tableId = LootTableConverter.ToId(table);
            var itemId = _pipeline.CraftFromDropTable(tableId);
            return _pipeline.GetItemData(itemId);
        }

        public IItem ClaimNextPendingItem()
        {
            var itemId = _pipeline.ClaimNextPendingItem();
            return _pipeline.GetItemData(itemId);
        }
        
        public void DeleteItem(Guid itemId) => _pipeline.DeleteItem(itemId);

        public IItem UpgradeItem(Guid itemId) => _pipeline.UpgradeItem(itemId);

        public string GetOwnerDescription(Guid ownerId) => _ownerRegistry.GetDescription(ownerId);
        #endregion

        #region Battle System Sockets

        public IReadOnlyList<IResolutionPayload> ActivateTrigger(TriggerContext context)
        {
            IReadOnlyList<ResolutionContext> internalResults = _effects.ActivateTrigger(context);
            return ResolutionPayloadMapper.Map(internalResults);
        }

        public void RollFromLootTable(LootTable table)
        {
            string tableId = LootTableConverter.ToId(table);
            _pipeline.RollFromDropTable(tableId);
        }
        #endregion

        private void PublishInventoryResolution(IReadOnlyList<ResolutionContext> internalResults)
        {
            if (internalResults == null || internalResults.Count == 0) return;

            IReadOnlyList<IResolutionPayload> payloads = ResolutionPayloadMapper.Map(internalResults);
            if (payloads.Count == 0) return;

            OnInventoryResolved?.Invoke(payloads);
        }
    }
}