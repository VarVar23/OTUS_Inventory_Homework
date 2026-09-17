using System.Collections.Generic;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryLoadService
    {
        [Inject] private InventoryView _view;
        [Inject] private InventoryRepository _repository;
        [Inject] private InventoryMultiCellService _multiCellService;

        public void Initialize()
        {
            InventoryCellView multiCell = null;

            for (int i = 0; i < _view.AllCells.Count; i++)
            {
                if (_view.AllCells[i].CellType == InventoryCellType.Crafting)
                {
                    multiCell = _view.AllCells[i];
                }
            }

            _repository.InitializeCells(_view.AllCells);
            _multiCellService.Load(multiCell, null);
        }

        public bool TryUpdateItemData(string itemID, InventoryPayloadData payload)
        {
            var item = _repository.GetItemViewByID(itemID);
            if (item == null) return false;

            item.Data = payload;
            _multiCellService.UpdateData();
            return true;
        }
    }
}
