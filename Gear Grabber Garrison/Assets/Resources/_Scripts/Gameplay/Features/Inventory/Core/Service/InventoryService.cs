using System;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryService : IInitializable
    {
        public event Action OnGetItemButtonClick
        {
            add => _visibilityService.OnGetItemButtonClick += value;
            remove => _visibilityService.OnGetItemButtonClick -= value;
        }

        public bool IsFull => _visibilityService.IsFull;

        [Inject] private InventoryLoadService _loadService;
        [Inject] private InventoryVisibilityService _visibilityService;
        [Inject] private InventoryItemInteractionService _interactionService;
        [Inject] private InventoryMultiCellService _multiCellService;

        public void Initialize()
        {
            _loadService.Initialize();
            _interactionService.Initialize();
            _multiCellService.Initialize();
            _multiCellService.OnDestroyRequest += _interactionService.UnSubscribeItem;
        }

        public bool TryUpdateItemData(string itemID, InventoryPayloadData payload)
        {
            return _loadService.TryUpdateItemData(itemID, payload);
        }

        public void UpdateCountAvailableItems(int count) => _visibilityService.UpdateCountAvailableItems(count);

        public void InitializeItem(InventoryItemView item) => _interactionService.InitializeItem(item);

        public void UnSubscribeItem(InventoryItemView item) => _interactionService.UnSubscribeItem(item);

        public void UpdateUpgradeVisuals(long currentBalance) => _visibilityService.UpdateUpgradeVisuals(currentBalance);
    }
}