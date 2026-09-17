using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryRepository
    {
        public List<InventorySlot> AllSlots = new();
        public bool IsFull => GetIsFull(); 

        private readonly Dictionary<InventoryCellView, InventorySlot> _cellToSlot = new();
        private readonly Dictionary<InventoryItemView, InventorySlot> _itemToSlot = new();

        public void InitializeCells(List<InventoryCellView> cellViews)
        {
            AllSlots.Clear();
            _cellToSlot.Clear();
            _itemToSlot.Clear();

            for (int i = 0; i < cellViews.Count; i++)
            {
                var cell = cellViews[i];
                var slot = new InventorySlot(cell, null);

                AllSlots.Add(slot);
                _cellToSlot[cell] = slot;
            }
        }

        public void RegisterCells(InventoryCellView[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                var slot = new InventorySlot(cells[i], null);
                AllSlots.Add(slot);
                _cellToSlot[cells[i]] = slot;
            }
        }

        public void TrySetItem(InventoryItemView item, InventoryCellView cell)
        {
            if (cell == null || !_cellToSlot.TryGetValue(cell, out var slot)) return;

            if (item != null && _itemToSlot.TryGetValue(item, out var oldSlot))
            {
                oldSlot.ItemView = null;
            }

            if (slot.ItemView != null)
            {
                _itemToSlot.Remove(slot.ItemView);
            }

            slot.ItemView = item;

            if (item != null)
            {
                _itemToSlot[item] = slot;
            }
        }

        public InventoryItemView GetItemViewByID(string itemID)
        {
            for(int i = 0; i < _cellToSlot.Count; i++)
            {
                if(AllSlots[i].ItemView == null) continue;

                if (AllSlots[i].ItemView.Data.GUID == itemID)
                {
                    return AllSlots[i].ItemView;
                }
            }

            return null;
        }

        public InventoryCellView GetCellByItem(InventoryItemView item)
        {
            if (item != null && _itemToSlot.TryGetValue(item, out var slot))
                return slot.CellView;
            return null;
        }

        public InventoryItemView GetItemByCell(InventoryCellView cell)
        {
            if (cell != null && _cellToSlot.TryGetValue(cell, out var slot))
                return slot.ItemView;
            return null;
        }

        public InventorySlot GetSlotByCell(InventoryCellView cell)
        {
            if (cell != null && _cellToSlot.TryGetValue(cell, out var slot))
                return slot;
            return null;
        }

        public InventorySlot GetSlotByItem(InventoryItemView item)
        {
            if (item != null && _itemToSlot.TryGetValue(item, out var slot))
                return slot;
            return null;
        }

        public InventoryCellView GetFirstEmptyCell()
        {
            for (int i = 0; i < AllSlots.Count; i++)
            {
                if (AllSlots[i].IsAvailable() && AllSlots[i].CellView.CellType == InventoryCellType.Bag) return AllSlots[i].CellView;
            }
            return null;
        }

        private bool GetIsFull()
        {
            for (int i = 0; i < AllSlots.Count; i++)
            {
                if (AllSlots[i].IsAvailable() && AllSlots[i].CellView.CellType == InventoryCellType.Bag) return false;
            }

            return true;
        }
    }
}