namespace Gameplay.Inventory
{
    internal class InventoryTooltipController
    {
        private InventoryTooltipInfoView _itemView;
        private InventoryTooltipUnitView _tooltipUnitView;
        private InventoryRepository _repository;

        public InventoryTooltipController(InventoryTooltipInfoView itemView, InventoryTooltipUnitView tooltipUnitView, InventoryRepository repository)
        {
            _itemView = itemView;
            _tooltipUnitView = tooltipUnitView;
            _repository = repository;
        }

        public void SetItemInfo(InventoryItemView view)
        {
            if (_repository.GetCellByItem(view).CellType == InventoryCellType.Crafting) return;

            _itemView.Active(true);
            _itemView.SetInfo(view.Data.Description);
            _itemView.SetPosition(view.transform);
        }

        public void SetUnitInfo(UnitView view, string info, string unitName)
        {
            _tooltipUnitView.Active(true);
            _tooltipUnitView.SetInfo(info, unitName);
            _tooltipUnitView.SetPosition(view.GetAvaTransform());
        }

        public void HideItem()
        {
            _itemView.Active(false);
        }

        public void HideUnit()
        {
            _tooltipUnitView.Active(false);
        }
    }
}