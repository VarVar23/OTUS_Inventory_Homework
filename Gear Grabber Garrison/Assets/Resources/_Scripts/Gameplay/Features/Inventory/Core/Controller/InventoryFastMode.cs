using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryFastMode
    {
        private InventoryRepository _inventoryRepository;
        private InventoryUnitsRepository _unitsRepository;
        private InventoryMultiCellRepository _multiCellRepository;

        public InventoryFastMode(InventoryRepository inventoryRepository, InventoryUnitsRepository unitsRepository, InventoryMultiCellRepository multiCellRepository)
        {
            _inventoryRepository = inventoryRepository;
            _unitsRepository = unitsRepository;
            _multiCellRepository = multiCellRepository;
        }

        public InventoryCellView GetTargetCell(InventoryItemView item)
        {
            var slot = _inventoryRepository.GetSlotByItem(item);

            InventoryCellView targetCell = null;

            if (slot.CellView.CellType == InventoryCellType.Bag)
            {
                targetCell = _unitsRepository.GetFirstEmptyCell(item.Data.Type);

                if (targetCell == null)
                    targetCell = _multiCellRepository.Slot.CellView;

                if (targetCell == null)
                    targetCell = _inventoryRepository.GetFirstEmptyCell();
            }
            else if (slot.CellView.CellType == InventoryCellType.Crafting)
            {
                targetCell = _unitsRepository.GetFirstEmptyCell(item.Data.Type);

                if (targetCell == null)
                    targetCell = _inventoryRepository.GetFirstEmptyCell();
            }
            else
            {
                targetCell = _inventoryRepository.GetFirstEmptyCell();
            }

            return targetCell;
        }
    }
}