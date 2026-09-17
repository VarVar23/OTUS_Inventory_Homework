using System;
using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;
using UnityEngine;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemPipelineManager
    {
        [Inject] private readonly JSONItemizationRegistry _registry;
        [Inject] private readonly ItemPool _pool;
        [Inject] private readonly BuildInstructionsGenerator _instructionsGenerator;
        [Inject] private readonly ItemBuilderService _builder;
        [Inject] private readonly ItemUpgradeService _upgradeService;

        public event Action<int> OnUnclaimedQueueUpdate
        {
            add => _pool.OnUnclaimedQueueUpdate += value;
            remove => _pool.OnUnclaimedQueueUpdate -= value;
        }

        public void RollFromDropTable(string tableId)
        {
            DropTableModel table = _registry.GetDropTable(tableId);
            if (table == null) return;

            if (WeightedRandomService.TryPick(table, out string blueprintId))
            {
                ExecuteGenerationPipeline(blueprintId, bypassQueue: false);
            }
            else
            {
                Debug.Log("<color=yellow>[Engine]</color> No item dropped from table: " + tableId);
            }
        }

        public Guid CraftFromDropTable(string tableId)
        {
            DropTableModel table = _registry.GetDropTable(tableId);
            if (table == null || table.NoItemDropWeight > 0)
            {
                Debug.LogError($"[Engine] Table {tableId} is not a valid Craft Table.");
                return Guid.Empty;
            }
            
            if (WeightedRandomService.TryPick(table, out string blueprintId))
            {
                return ExecuteGenerationPipeline(blueprintId, bypassQueue: true);
            }
            
            return Guid.Empty;
        }

        public Guid ClaimNextPendingItem()
        {
            if (_pool.UnclaimedItemCount == 0) return Guid.Empty;

            Item item = _pool.PopNextGenerated();
            return item.Id;
        }

        public void DeleteItem(Guid id)
        {
            _pool.DestroyItem(id);
        }

        public Item GetItemData(Guid id) => _pool.GetItem(id);

        public Item UpgradeItem(Guid itemId)
        {
            Item item = _pool.GetItem(itemId);
            if (item != null)
            {
                _upgradeService.UpgradeItem(item);
            }
            return item;
        }

        private Guid ExecuteGenerationPipeline(string blueprintId, bool bypassQueue)
        {
            ItemBlueprintModel blueprint = _registry.GetBlueprint(blueprintId);
            
            BuildInstructions instructions = _instructionsGenerator.CreateInstructions(blueprint);

            Item newItem = _builder.BuildFromInstructions(instructions);

            if (bypassQueue)
                _pool.RegisterItemWithStatus(newItem, ItemStatus.Received);
            else
                _pool.RegisterItem(newItem);

            return newItem.Id;
        }
    }
}