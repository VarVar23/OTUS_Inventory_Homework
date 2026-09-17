using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryUnitsRepository
    {
        public event Action<Guid, string> OnEquippedItem;
        public event Action<Guid, string> OnUnequippedItem;

        private readonly InventoryRepository _repository;
        private readonly Dictionary<Guid, UnitPayloadData> _units = new();
        private readonly Dictionary<Guid, UnitView> _unitViews = new();
        private readonly Dictionary<InventoryCellView, Guid> _cellToUnitId = new();
        private readonly List<Guid> _unlockedUnits = new();

        public InventoryUnitsRepository(InventoryRepository repository)
        {
            _repository = repository;
        }

        public void UnlockUnit(Guid unitId)
        {
            if (!_unlockedUnits.Contains(unitId))
                _unlockedUnits.Add(unitId);

            GetView(unitId).Unlock(true);
        }

        public void Load(List<UnitPayloadData> units)
        {
            _units.Clear();
            for(int i = 0; i < units.Count; i++)
            {
                _units.Add(units[i].ID, units[i]);
            }
        }

        public void RegisterView(UnitView view, UnitPayloadData data)
        {
            _unitViews.Add(data.ID, view);
            _units.Add(data.ID, data);

            foreach (var cell in view.CellViews)
            {
                _cellToUnitId[cell] = data.ID;
            }
        }

        public Dictionary<Guid, List<string>> GetEquippedGUIDItems()
        {
            var result = new Dictionary<Guid, List<string>>();

            foreach (var pair in _unitViews)
            {
                var guidList = new List<string>();

                foreach (var cell in pair.Value.CellViews)
                {
                    var item = _repository.GetItemByCell(cell);
                    if (item != null)
                    {
                        guidList.Add(item.Data.GUID);
                    }
                }

                result.Add(pair.Key, guidList);
            }

            return result;
        }

        public void TrySetItem(InventoryItemView item, InventoryCellView cell)
        {
            if (!IsEquipCell(cell)) return;
            if (!_cellToUnitId.TryGetValue(cell, out var unitId)) return;

            var currentItem = _repository.GetItemByCell(cell);

            if (currentItem != null)
            {
                OnUnequippedItem?.Invoke(unitId, currentItem.Data.GUID);
            }

            if (item != null)
            {
                OnEquippedItem?.Invoke(unitId, item.Data.GUID);
            }
        }

        public InventoryCellView GetFirstEmptyCell(InventoryCellType type)
        {
            foreach (var pair in _unitViews)
            {
                foreach (var cell in pair.Value.CellViews)
                {
                    var slot = _repository.GetSlotByCell(cell);

                    if (slot != null && cell.CellType == type && slot.ItemView == null && cell.gameObject.activeInHierarchy)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        public List<Guid> GetUnlockedUnitIDs() => new(_unlockedUnits);
        public UnitPayloadData GetUnitData(Guid unitId) => _units[unitId];
        public UnitView GetView(Guid unitId) => _unitViews[unitId];
        public void SetUnitEffects(Guid unitId, string effects) => _units[unitId].Effects = effects;
        public void SetUnitName(Guid unitId, string name) => _units[unitId].Name = name;
        private bool IsEquipCell(InventoryCellView cell) => cell.CellType != InventoryCellType.Bag && cell.CellType != InventoryCellType.Crafting;
    }
}