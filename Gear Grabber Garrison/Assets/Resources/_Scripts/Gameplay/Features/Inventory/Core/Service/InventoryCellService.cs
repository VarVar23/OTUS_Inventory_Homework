using System;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryCellService
    {
        public event Action<Transform, Transform> MoveToCell;
        private InventoryRepository _repository;
        private InventoryUnitsRepository _unitsRepository;
        private InventoryMultiCellRepository _multiCellRepository;
        private InventorySettings _settings;

        public InventoryCellService(InventoryRepository repository, InventoryUnitsRepository unitsRepository, InventoryMultiCellRepository multiCellRepository, InventorySettings settings)
        {
            _repository = repository;
            _unitsRepository = unitsRepository;
            _multiCellRepository = multiCellRepository;
            _settings = settings;
        }

        public void PutItem(InventoryCellView targetCell, InventoryItemView itemView)
        {
            var lastCell = _repository.GetCellByItem(itemView);
            var swapItem = _repository.GetItemByCell(targetCell);
            var targetSlot = _repository.GetSlotByCell(targetCell);
    
            if (targetCell == null || (!targetSlot.IsAvailable() && !_settings.SwapItems) || !targetCell.gameObject.activeInHierarchy)
            {
                MoveToCell?.Invoke(itemView.transform, lastCell.transform);
                return;
            }

            if (_settings.SwapItems && swapItem != null)
            {
                MoveToCell?.Invoke(swapItem.transform, lastCell.transform);
                SetItem(null, targetCell);
                SetItem(swapItem, lastCell);
            }
            else
            {
                SetItem(null, lastCell);
            }

            MoveToCell?.Invoke(itemView.transform, targetCell.transform);
            SetItem(itemView, targetCell);
        }

        public void SetItem(InventoryItemView item, InventoryCellView view)
        {
            _unitsRepository.TrySetItem(item, view);
            _repository.TrySetItem(item, view);

            if(view.CellType == InventoryCellType.Crafting)
                _multiCellRepository.SetItem(item);
        }
    }
}