using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryCellFinder
    {
        private InventorySettings _settings;
        private InventoryRepository _repository;

        public InventoryCellFinder(InventorySettings settings, InventoryRepository repository)
        {
            _settings = settings;
            _repository = repository;
        }

        public InventoryCellView FindNearestCell(InventoryItemView itemView)
        {
            InventoryCellView resultCell = null;
            Vector3 itemPosition = itemView.transform.position;
            float minDistance = float.MaxValue;

            for (int i = 0; i < _repository.AllSlots.Count; i++)
            {
                var cellView = _repository.AllSlots[i].CellView;
                float distance = (cellView.transform.position - itemPosition).magnitude;

                if (distance < _settings.SnapDistance && IsSameType(cellView, itemView))
                {
                    if (distance < minDistance)
                    {
                        var itemInCell = _repository.GetItemByCell(cellView);
                        var lastCell = _repository.GetCellByItem(itemView);

                        if (itemInCell != null && _settings.SwapItems && !IsSameType(lastCell, itemInCell))
                            continue;

                        resultCell = cellView;
                        minDistance = distance;
                    }
                }
            }

            return resultCell;
        }

        public bool IsSameType(InventoryCellView cellView, InventoryItemView itemView)
        {
            if (cellView.CellType == InventoryCellType.Bag || cellView.CellType == InventoryCellType.Crafting) return true;
            return cellView.CellType == itemView.Data.Type;
        }
    }
}