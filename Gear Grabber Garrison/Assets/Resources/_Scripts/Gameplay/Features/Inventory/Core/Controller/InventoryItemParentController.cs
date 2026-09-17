using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryItemParentController
    {
        private InventoryView _view;

        public InventoryItemParentController(InventoryView view)
        {
            _view = view;
        }

        public void SetPriorityParent(InventoryItemView inventoryItem)
        {
            inventoryItem.transform.SetParent(_view.PriorityItemParent);
        }

        public void SetPriorityParent(Transform item)
        {
            item.SetParent(_view.PriorityItemParent);
        }

        public void SetParent(Transform item, Transform parent)
        {
            item.SetParent(parent);
        }
    }
}