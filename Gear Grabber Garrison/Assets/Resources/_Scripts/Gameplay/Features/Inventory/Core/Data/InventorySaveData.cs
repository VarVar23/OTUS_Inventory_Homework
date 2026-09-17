using System;
using System.Collections.Generic;

namespace Gameplay.Inventory
{
    [Serializable]
    public class InventorySaveData
    {
        public List<ItemSlotData> Items = new();
        public List<string> UnlockedUnits = new(); // Temp
    }

    [Serializable]
    public class ItemSlotData
    {
        public string ItemID;
        public int CellIndex;
    }
}
