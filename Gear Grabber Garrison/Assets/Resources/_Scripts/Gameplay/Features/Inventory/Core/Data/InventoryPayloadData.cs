using System;
using UnityEngine;

namespace Gameplay.Inventory
{
    [Serializable]
    public class InventoryPayloadData
    {
        public Sprite Icon;
        public InventoryCellType Type;
        public int UpgradePrice;
        public int DestroyPrice;
        public bool CanUpgrade;
        public string GUID;
        public string Description;
        public string UpgradeDescription;
    }
}