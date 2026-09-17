using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public interface IInventory
    {
        public IInventoryUnit Unit { get; }
        
        public event Action OnGenerateButtonClick;
        public event Action OnGetItemButtonClick;
        public event Action OnMultiCellItemUpdate;

        public Action<string> OnUpgradeButtonClick { get; set; }
        public Action<string> OnDestroyButtonClick { get; set; }
        
        public bool IsFull { get; }
        public InventorySaveData GetSaveData();
        public InventoryPayloadData GetMultiCellItemData();
        public void Load(InventorySaveData data, List<InventoryPayloadData> payloads);
        public void UpdateCountAvailableItems(int count);
        public void SetGeneratePrice(long price);
        public void SetGeneratePriceColor(Color color);
        public void SetUpgradePriceColor(Color color);
        public bool TryUpdateItemData(string itemID, InventoryPayloadData payload);
        public bool TryPutGeneratedItem(InventoryPayloadData payload);
        public bool TryPutItem(InventoryPayloadData payload);
        public void UpdateUpgradeVisuals(long currentBalance);
    }
}