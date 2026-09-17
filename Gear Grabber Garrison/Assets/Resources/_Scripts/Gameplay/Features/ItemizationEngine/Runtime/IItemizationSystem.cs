using System;
using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization
{
    public interface IItemizationSystem
    {
        public event Action<int> OnUnclaimedQueueUpdate;
        public event Action<IReadOnlyList<IResolutionPayload>> OnInventoryResolved;

        public void EquipItem(Guid ownerId, Guid itemId);
        public void UnequipItem(Guid ownerId, Guid itemId);
        public IReadOnlyList<IResolutionPayload> ActivateTrigger(TriggerContext context);
        public void RollFromLootTable(LootTable table);
        public IItem CraftFromLootTable(LootTable table);
        public IItem ClaimNextPendingItem();   
        public void DeleteItem(Guid itemId);
        public IItem GetItemData(Guid itemId);
        public IItem UpgradeItem(Guid itemId);
        public string GetOwnerDescription(Guid ownerId);
    }
}