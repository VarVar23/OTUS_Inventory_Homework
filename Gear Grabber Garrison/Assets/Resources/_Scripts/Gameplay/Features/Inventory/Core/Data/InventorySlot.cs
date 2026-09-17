namespace Gameplay.Inventory
{
    internal class InventorySlot
    {
        public InventoryCellView CellView;
        public InventoryItemView ItemView;

        public InventorySlot(InventoryCellView cellView, InventoryItemView itemView)
        {
            CellView = cellView;
            ItemView = itemView;
        }

        public bool IsAvailable() => ItemView == null;
    }
}