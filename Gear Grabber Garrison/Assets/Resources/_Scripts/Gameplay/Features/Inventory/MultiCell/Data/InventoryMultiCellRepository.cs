using System;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryMultiCellRepository
    {
        public event Action<InventoryItemView> OnMultiCellSet;
        public InventorySlot Slot { get; private set; }

        public void Load(InventoryCellView cell, InventoryItemView item)
        {
            Slot = new(cell, item);

            OnMultiCellSet?.Invoke(item);
        }

        public void SetItem(InventoryItemView item)
        {
            Slot.ItemView = item;
            OnMultiCellSet?.Invoke(item);
        }
    }
}