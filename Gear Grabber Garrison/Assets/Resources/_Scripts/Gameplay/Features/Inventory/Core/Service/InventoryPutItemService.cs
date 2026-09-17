using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryPutItemService
    {
        [Inject] private InventoryRepository _repository;
        [Inject] private InventoryBuilder _builder;
        [Inject] private InventoryService _service;
        [Inject] private InventoryMoveService _moveService;
        [Inject] private InventoryCellService _cellService;
        [Inject] private InventoryItemParentController _parentController;
        [Inject] private InventoryMultiCellRepository _multiCellRepository;

        public bool TryPutGeneratedItem(InventoryPayloadData payload)
        {
            if (_multiCellRepository.Slot.ItemView != null) return false;

            return PutItem(payload, _multiCellRepository.Slot.CellView, false);
        }

        public bool TryPutItem(InventoryPayloadData payload)
        {
            var cell = _repository.GetFirstEmptyCell();
            if (cell == null) return false;

            return PutItem(payload, cell, true);
        }

        public bool TryPutItemToCell(InventoryPayloadData payload, InventoryCellView cell)
        {
            if (cell == null) return false;

            return PutItem(payload, cell, false);
        }

        private bool PutItem(InventoryPayloadData payload, InventoryCellView cell, bool animate)
        {
            var item = _builder.GenerateItem(payload);
            _service.InitializeItem(item);
            _cellService.SetItem(item, cell);
            
            if (animate)
            {
                _parentController.SetPriorityParent(item);
                _moveService.MoveAnimation(item.transform, cell.transform, () => _parentController.SetParent(item.transform, cell.transform));
            }
            else
            {
                _parentController.SetParent(item.transform, cell.transform);
                _moveService.MoveWithoutAnimation(item.transform, cell.transform.position);
            }

            return true;
        }
    }
}
