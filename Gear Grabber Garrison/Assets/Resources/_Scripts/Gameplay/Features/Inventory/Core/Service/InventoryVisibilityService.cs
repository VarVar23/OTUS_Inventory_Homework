using System;

namespace Gameplay.Inventory
{
    internal class InventoryVisibilityService
    {
        public event Action OnGetItemButtonClick
        {
            add => _addItemsView.OnAddItemsButtonClick += value;
            remove => _addItemsView.OnAddItemsButtonClick -= value;
        }

        public bool IsFull => _repository.IsFull;

        private InventoryView _view;
        private InventoryAddItemsView _addItemsView;
        private InventoryRepository _repository;

        public InventoryVisibilityService(InventoryView view, InventoryAddItemsView addItemsView, InventoryRepository repository)
        {
            _view = view;
            _addItemsView = addItemsView;
            _repository = repository;
        }

        public void UpdateCountAvailableItems(int count) => _addItemsView.SetCount(count);

        public void UpdateUpgradeVisuals(long currentBalance)
        {
            foreach (var slot in _repository.AllSlots)
            {
                if (slot.ItemView != null)
                {
                    bool canUpgrade = slot.ItemView.Data.CanUpgrade && currentBalance >= slot.ItemView.Data.UpgradePrice;
                    slot.ItemView.SetUpgradeVisual(canUpgrade);
                }
            }
        }
    }
}